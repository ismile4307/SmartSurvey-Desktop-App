# Checks the desktop API sign-in on the server, using the credentials saved in C:\Temp\index.ini.
# Prints HTTP status codes only - never the password or the token.
#   powershell -ExecutionPolicy Bypass -File "desk_api_check.ps1" [-Server https://smartsurveybd.com]
param([string]$Server = "https://smartsurveybd.com")

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$lines = Get-Content "C:\Temp\index.ini"
$email = $lines[0]; $pass = $lines[1]

function Post($path, $body, $headers) {
    $h = @{ Accept = "application/json" }
    if ($headers) { $headers.GetEnumerator() | ForEach-Object { $h[$_.Key] = $_.Value } }
    try {
        $r = Invoke-WebRequest -Uri "$Server$path" -Method Post -Body $body -Headers $h `
             -ContentType "application/x-www-form-urlencoded" -UseBasicParsing -TimeoutSec 120
        return @{ Status = [int]$r.StatusCode; Body = $r.Content }
    } catch {
        $resp = $_.Exception.Response
        if (-not $resp) { return @{ Status = -1; Body = $_.Exception.Message } }
        $sr = New-Object IO.StreamReader($resp.GetResponseStream())
        return @{ Status = [int]$resp.StatusCode; Body = $sr.ReadToEnd() }
    }
}

$sw = [Diagnostics.Stopwatch]::StartNew()
$login = Post "/api/desk/login" ("email=" + [Uri]::EscapeDataString($email) + "&password=" + [Uri]::EscapeDataString($pass))
"1. login                       : HTTP $($login.Status) ($([int]$sw.Elapsed.TotalSeconds)s)"
if ($login.Status -ne 200) { "   " + $login.Body; exit 1 }
$token = ($login.Body | ConvertFrom-Json).token
"   token has 'id|' prefix      : $(if ($token -like '*|*') { 'yes (server DeskAuthController not updated)' } else { 'no' })"
$secret = if ($token -like '*|*') { ($token -split '\|', 2)[1] } else { $token }
$field = "api_token=" + [Uri]::EscapeDataString($secret)

$r = Post "/api/desk/projects" "" @{ Authorization = "Bearer $secret" }
"2. projects, header only       : HTTP $($r.Status)"
$r = Post "/api/desk/projects" $field $null
"3. projects, body only         : HTTP $($r.Status)"
$r = Post "/api/desk/projects" $field @{ Authorization = "Bearer $secret" }
"4. projects, header + body     : HTTP $($r.Status)   <- what the app sends"
""
"2 = 401 and 3 = 200 : Apache drops the Authorization header; the body fallback handles it."
"3 = 401             : DeskTokenFromBody is not deployed yet (or the token itself is rejected)."
