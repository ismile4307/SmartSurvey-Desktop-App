<?php
$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_capi";



// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
if (isset($_POST['ProjectId'])) {
    $projectId = $_POST['ProjectId'];
}
if (isset($_POST['RespondentId'])) {
    $RespondentId = $_POST['RespondentId'];
}
if (isset($_POST['Latitude'])) {
    $Latitude= $_POST['Latitude'];
}
if (isset($_POST['Longitude'])) {
    $Longitude= $_POST['Longitude'];
}
if (isset($_POST['SurveyDateTime'])) {
    $SurveyDateTime = date('Y-m-d H:i:s', strtotime($_POST['SurveyDateTime']));
}
if (isset($_POST['SurveyEndTime'])) {
    $SurveyEndTime = date('Y-m-d H:i:s', strtotime($_POST['SurveyEndTime']));
}
if (isset($_POST['LengthOfIntv'])) {
    $LengthOfIntv = $_POST['LengthOfIntv'];
}
if (isset($_POST['Intv_Type'])) {
    $Intv_Type = $_POST['Intv_Type'];
}
if (isset($_POST['FICode'])) {
    $FICode = $_POST['FICode'];
}
if (isset($_POST['FSCode'])) {
    $FSCode = $_POST['FSCode'];
}
if (isset($_POST['AccompaniedBy'])) {
    $AccompaniedBy = $_POST['AccompaniedBy'];
}
if (isset($_POST['BackCheckedBy'])) {
    $BackCheckedBy = $_POST['BackCheckedBy'];
}
if (isset($_POST['Status'])) {
    $Status = $_POST['Status'];
}
if (isset($_POST['TabId'])) {
    $TabId = $_POST['TabId'];
}
if (isset($_POST['SyncStatus'])) {
    $SyncStatus = $_POST['SyncStatus'];
}
if (isset($_POST['ScriptVersion'])) {
    $ScriptVersion = $_POST['ScriptVersion'];
}
if (isset($_POST['LanguageId'])) {
    $LanguageId = $_POST['LanguageId'];
}
if (isset($_POST['FieldExtra1'])) {
    $FieldExtra1 = $_POST['FieldExtra1'];
}
if (isset($_POST['FieldExtra2'])) {
    $FieldExtra2 = $_POST['FieldExtra2'];
}
if (isset($_POST['SyncDateTime'])) {
    $DateOfSync = $_POST['SyncDateTime'];
}
if (isset($_POST['QId'])) {
    $arrayQId = $_POST['QId'];
}
if (isset($_POST['Response'])) {
    $arrayResponse = $_POST['Response'];
}
if (isset($_POST['ResponseDateTime'])) {
    $arrayResponseDateTime = $_POST['ResponseDateTime'];//date('Y-m-d H:i:s', strtotime($_POST['ResponseDateTime']));

}
if (isset($_POST['qElapsedTime'])) {
    $arrayqElapsedTime = $_POST['qElapsedTime'];
}
if (isset($_POST['qOrderTag'])) {
    $arrayqOrderTag = $_POST['qOrderTag'];
}
if (isset($_POST['rOrderTag'])) {
    $arrayrOrderTag = $_POST['rOrderTag'];
}
if (isset($_POST['QIdOE'])) {
    $arrayQIdOE = $_POST['QIdOE'];
}
if (isset($_POST['AttributeValue'])) {
    $arrayAttributeValue = $_POST['AttributeValue'];
}
if (isset($_POST['OpenendedResp'])) {
    $arrayOpenendedResp = $_POST['OpenendedResp'];
}
if (isset($_POST['OEResponseType'])) {
    $arrayTypeOfOE = $_POST['OEResponseType'];
}

$AutoId=0;

$recExist = "SELECT * FROM interview_infos WHERE respondent_id=" . $RespondentId;

if (mysqli_query($conn, $recExist)) {
    $noOfRecord = mysqli_num_rows(mysqli_query($conn, $recExist));
    //echo $noOfRecord;
    if ($noOfRecord > 0) {
        //$update = mysqli_query($conn, "UPDATE interview_infos SET deleted_at='" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "' WHERE respondent_id=" . $RespondentId );

        //if ($update) {
        //    //throw new Exception("Update successful");
        //    //echo "Update successful";
        //} else {
        //    //echo "Error: " . $update . "<br>" . $conn->error;
        //    echo "Update error";
        //}
        echo "Record already exists";
    } else {
        //Insert
        //echo "insert success";
        
        //*****************************************************************************


//************ Insert into T_InterviewInfo *******************
$sql = "INSERT INTO `interview_infos`(`project_id`, `respondent_id`, `latitude`, `longitude`, `survey_start_at`, `survey_end_at`, `length_of_intv`, `intv_type`, `fi_code`, `fs_code`, `accompanied_by`, `back_checked_by`, `status`, `tab_id`, `sync_status`, `script_version`, `language_id`, `field_ex1`, `field_ex2`, `created_at`, `deleted_at`)
VALUES (" . $projectId . "," . $RespondentId . ",'" . $Latitude . "','" . $Longitude . "','" . $SurveyDateTime . "','" . $SurveyEndTime . "','" . $LengthOfIntv . "','" . $Intv_Type . "','" . $FICode . "','" . $FSCode . "','" . $AccompaniedBy . "','" . $BackCheckedBy . "','" . $Status . "','" . $TabId . "','" . $SyncStatus . "','" . $ScriptVersion . "','" . $LanguageId . "','" . $FieldExtra1 . "','" . $FieldExtra2 . "','" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "', NULL)";

if ($conn->query($sql) === TRUE) {
    //echo "New record created successfully";
    //echo json_encode(array('message'=>"New record created successfully"));
    $AutoId=mysqli_insert_id($conn);
} else {
    echo "Error: " . $sql . "<br>" . $conn->error;
}
//************ Insert into T_RespAnswer *******************
for ($i = 0; $i < sizeof($arrayQId); $i++) {
    //var_dump($arrayResponseDateTime);
    //debug_to_console($arrayResponseDateTime);
    $sql = "INSERT INTO `answers`(`interview_info_id`, `project_id`, `respondent_id`, `q_id`, `response`, `responded_at`, `q_elapsed_time`, `q_order`, `resp_order`, `created_at`, `deleted_at`)
           VALUES (" . $AutoId . "," . $projectId . "," . $RespondentId . ",'" . $arrayQId[$i] . "','" . $arrayResponse[$i] . "','" . date('Y-m-d H:i:s', strtotime($arrayResponseDateTime[$i])) . "','" . $arrayqElapsedTime[$i] . "'," . $arrayqOrderTag[$i] . "," . $arrayrOrderTag[$i] . ",'" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "','')";
    if ($conn->query($sql) === TRUE) {
        //echo "New record created successfully";
        //echo json_encode(array('message'=>"New record created successfully"));
    } else {
        echo "Error: " . $sql . "<br>" . $conn->error;
    }
}
//************ Insert into T_Openended *******************
if (sizeof($arrayQIdOE) >0 && $arrayQIdOE[0]!='') {
    //echo sizeof($arrayQIdOE);
    for ($i = 0; $i < sizeof($arrayQIdOE); $i++) {
        //var_dump($arrayResponseDateTime);
        //debug_to_console($arrayResponseDateTime);
        $sql = "INSERT INTO `open_endeds`(`interview_info_id`, `project_id`, `respondent_id`, `q_id`, `attribute_value`, `response`, `response_type`, `created_at`, `deleted_at`)
           VALUES (" . $AutoId . "," . $projectId . "," . $RespondentId . ",'" . $arrayQIdOE[$i] . "','" . $arrayAttributeValue[$i] . "','" . $arrayOpenendedResp[$i] . "','" . $arrayTypeOfOE[$i] . "','" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "','')";
        if ($conn->query($sql) === TRUE) {
            //echo "New record created successfully";
            //echo json_encode(array('message'=>"New record created successfully"));
        } else {
            echo "Error: " . $sql . "<br>" . $conn->error;
        }
    }
}
//if ($conn->query($sql) === TRUE) {
echo "New record created successfully";
//echo json_encode(array('message'=>"New record created successfully"));
//} else {
//    echo "Error: " . $sql . "<br>" . $conn->error;
//}

//**********************************************************


    }
} else {
    echo "Error: " . $recExist . "<br>" . $conn->error;
    //echo "Rec exist error";
}




$conn->close();
/*$projectId = $_REQUEST['ProjectId'];
for($i=0; $i< count($projectId); $i++){
    echo $projectId[$i];
}
*/
?>