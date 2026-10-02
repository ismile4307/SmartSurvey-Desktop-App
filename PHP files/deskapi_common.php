<?php
// Shared helpers for the desktop download endpoints
// (respondentbyproject.php, answerbyproject.php, openendedbyproject.php).
//
// Deploy this file next to the endpoints, together with db_config.php
// (copy db_config.sample.php and fill in the real credentials — never commit it).

function desk_fail($message)
{
    if (!headers_sent()) {
        http_response_code(500);
        header('Content-Type: text/plain; charset=utf-8');
    }
    echo 'Error: ' . $message;
    exit;
}

function desk_connect()
{
    $configFile = __DIR__ . '/db_config.php';
    if (!is_file($configFile)) {
        desk_fail('db_config.php is missing in ' . __DIR__ . ' (copy db_config.sample.php and fill in the credentials)');
    }
    $cfg = require $configFile;
    if (!is_array($cfg) || !isset($cfg['host'], $cfg['user'], $cfg['pass'], $cfg['name'])) {
        desk_fail('db_config.php must return array(host, user, pass, name) — see db_config.sample.php');
    }
    $connection = mysqli_connect($cfg['host'], $cfg['user'], $cfg['pass'], $cfg['name']);
    if (!$connection) {
        desk_fail('Database connection failed: ' . mysqli_connect_error());
    }
    return $connection;
}

function desk_post($name, $default = '')
{
    return isset($_POST[$name]) ? (string)$_POST[$name] : $default;
}

/**
 * Reads and sanitises the common request parameters.
 *
 * Paging:
 *  - lastId + pageSize (new desktop client): keyset paging, "id > lastId ORDER BY id LIMIT pageSize".
 *    Deterministic and just as fast on the last page as on the first.
 *  - myOffset (older clients): the previous LIMIT/OFFSET behaviour, now with a stable ORDER BY.
 *  - neither: everything in one response (older clients of respondents / open-ended).
 */
function desk_params($connection)
{
    $projectCode = desk_post('projectCode');
    if (!preg_match('/^[0-9]+$/', $projectCode)) {
        desk_fail('Invalid projectCode');   // it is used as part of a table name
    }

    $p = array();
    $p['projectCode']   = $projectCode;
    $p['startDate']     = mysqli_real_escape_string($connection, desk_post('startDate') . ' 00:00:00');
    $p['endDate']       = mysqli_real_escape_string($connection, desk_post('endDate') . ' 23:59:59');
    $p['dateType']      = desk_post('dateType') === '1' ? '1' : '2';   // 1 = interview date, 2 = sync date
    $p['interviewType'] = mysqli_real_escape_string($connection, desk_post('interviewType'));
    $p['keyset']        = isset($_POST['lastId']);
    $p['lastId']        = max(0, (int)desk_post('lastId', '0'));
    $p['pageSize']      = min(50000, max(1, (int)desk_post('pageSize', '10000')));
    $p['offset']        = max(0, (int)desk_post('myOffset', '0'));

    if ($p['keyset']) {
        // Tells the desktop app this server pages by id, so it can stop on a short page.
        header('X-Desk-Paging: keyset');
    }
    return $p;
}

/** WHERE conditions on interview_infos_<code> shared by all three endpoints. */
function desk_interview_filter($p)
{
    $ii = 'interview_infos_' . $p['projectCode'];
    $dateColumn = $p['dateType'] === '1' ? 'survey_start_at' : 'created_at';
    return "$ii.project_id=" . $p['projectCode']
         . " AND $ii.$dateColumn BETWEEN '" . $p['startDate'] . "' AND '" . $p['endDate'] . "'"
         . " AND $ii.intv_type='" . $p['interviewType'] . "'"
         . " AND $ii.`status`!='4'"
         . " AND $ii.deleted_at IS NULL";
}

/**
 * Streams the query result as a JSON array, row by row, instead of building the
 * whole array in PHP memory first. Output is gzip-compressed when the client
 * accepts it (the desktop app does). A row with invalid UTF-8 no longer blanks
 * out the whole response.
 */
function desk_stream_json($connection, $query)
{
    $result = mysqli_query($connection, $query, MYSQLI_USE_RESULT);
    if (!$result) {
        desk_fail('Query failed: ' . mysqli_error($connection));
    }

    $flags = defined('JSON_INVALID_UTF8_SUBSTITUTE') ? JSON_INVALID_UTF8_SUBSTITUTE : JSON_PARTIAL_OUTPUT_ON_ERROR;

    header('Content-Type: application/json; charset=utf-8');
    if (!ob_start('ob_gzhandler')) {
        ob_start();
    }

    echo '[';
    $first = true;
    while ($row = mysqli_fetch_assoc($result)) {
        $json = json_encode($row, $flags);
        if ($json === false) {
            continue;
        }
        echo $first ? $json : ',' . $json;
        $first = false;
    }
    echo ']';

    mysqli_free_result($result);
    ob_end_flush();
}
