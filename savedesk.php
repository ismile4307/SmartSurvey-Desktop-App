<?php
error_reporting(0);
$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_smartsurvey";



// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
// Default all variables to empty string so INSERT never uses undefined variables
$projectId = ''; $RespondentId = ''; $Latitude = ''; $Longitude = '';
$SurveyDateTime = ''; $SurveyEndTime = ''; $LengthOfIntv = ''; $Intv_Type = '';
$FICode = ''; $FSCode = ''; $AccompaniedBy = ''; $BackCheckedBy = '';
$Status = ''; $TabId = ''; $SyncStatus = ''; $ScriptVersion = '';
$LanguageId = ''; $FieldExtra1 = ''; $FieldExtra2 = '';
$fi_name = ''; $fs_name = ''; $centre_code = ''; $name_resp = '';
$mobile_resp = ''; $address_resp = '';
$intv_info1 = ''; $intv_info2 = ''; $intv_info3 = ''; $intv_info4 = '';
$intv_info5 = ''; $intv_info6 = ''; $intv_info7 = ''; $intv_info8 = '';
$intv_info9 = ''; $intv_info10 = '';

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



//************************* Additional Field Data  **********************
if (isset($_POST['FIName'])) {
    $fi_name = $_POST['FIName'];
}
if (isset($_POST['FSName'])) {
    $fs_name = $_POST['FSName'];
}
if (isset($_POST['CentreCode'])) {
    $centre_code = $_POST['CentreCode'];
}
if (isset($_POST['NameResp'])) {
    $name_resp = $_POST['NameResp'];
}
if (isset($_POST['MobileResp'])) {
    $mobile_resp = $_POST['MobileResp'];
}
if (isset($_POST['AddressResp'])) {
    $address_resp = $_POST['AddressResp'];
}
if (isset($_POST['IntvInfo1'])) {
    $intv_info1 = $_POST['IntvInfo1'];
}
if (isset($_POST['IntvInfo2'])) {
    $intv_info2 = $_POST['IntvInfo2'];
}
if (isset($_POST['IntvInfo3'])) {
    $intv_info3 = $_POST['IntvInfo3'];
}
if (isset($_POST['IntvInfo4'])) {
    $intv_info4 = $_POST['IntvInfo4'];
}
if (isset($_POST['IntvInfo5'])) {
    $intv_info5 = $_POST['IntvInfo5'];
}
if (isset($_POST['IntvInfo6'])) {
    $intv_info6 = $_POST['IntvInfo6'];
}
if (isset($_POST['IntvInfo7'])) {
    $intv_info7 = $_POST['IntvInfo7'];
}
if (isset($_POST['IntvInfo8'])) {
    $intv_info8 = $_POST['IntvInfo8'];
}
if (isset($_POST['IntvInfo9'])) {
    $intv_info9 = $_POST['IntvInfo9'];
}
if (isset($_POST['IntvInfo10'])) {
    $intv_info10 = $_POST['IntvInfo10'];
}

////////////////////////////////////////////////

// if (isset($_POST['SyncDateTime'])) {
//     $DateOfSync = $_POST['SyncDateTime'];
// }


$DateOfSync = date('Y-m-d H:i:s');



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











$arrayQIdOE=null;

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

$recExist = "SELECT * FROM interview_infos_" . $projectId . " WHERE respondent_id=" . $RespondentId ." AND deleted_at IS NULL";

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

        //date('Y-m-d H:i:s', strtotime($DateOfSync))
        //************ Insert into T_InterviewInfo *******************
        $sql = "INSERT INTO `interview_infos_" . $projectId . "` (`project_id`, `respondent_id`, `latitude`, `longitude`, `survey_start_at`, `survey_end_at`, `length_of_intv`, `intv_type`, `fi_code`, `fs_code`, `accompanied_by`, `back_checked_by`, `status`, `tab_id`, `sync_status`, `script_version`, `language_id`, `field_ex1`, `field_ex2`, `fi_name`, `fs_name`, `centre_code`, `name_resp`, `mobile_resp`, `address_resp`, `intv_info1`, `intv_info2`, `intv_info3`, `intv_info4`, `intv_info5`, `intv_info6`, `intv_info7`, `intv_info8`, `intv_info9`, `intv_info10`, `created_at`, `deleted_at`)
        VALUES (" . $projectId . "," . $RespondentId . ",'" . $Latitude . "','" . $Longitude . "','" . $SurveyDateTime . "','" . $SurveyEndTime . "','" . $LengthOfIntv . "','" . $Intv_Type . "','" . $FICode . "','" . $FSCode . "','" . $AccompaniedBy . "','" . $BackCheckedBy . "','" . $Status . "','" . $TabId . "','" . $SyncStatus . "','" . $ScriptVersion . "','" . $LanguageId . "','" . $FieldExtra1 . "','" . $FieldExtra2 . "','" . $fi_name . "','" . $fs_name . "','" . $centre_code . "','" . $name_resp . "','" . $mobile_resp . "','" . $address_resp . "','" . $intv_info1 . "','" . $intv_info2 . "','" . $intv_info3 . "','" . $intv_info4 . "','" . $intv_info5 . "','" . $intv_info6 . "','" . $intv_info7 . "','" . $intv_info8 . "','" . $intv_info9 . "','" . $intv_info10 . "','" . $DateOfSync . "', NULL)";
        
        if ($conn->query($sql) === TRUE) {
            $AutoId=mysqli_insert_id($conn);
            //************ Insert into T_RespAnswer *******************
            for ($i = 0; $i < sizeof($arrayQId); $i++) {
                $sql = "INSERT INTO `answers_" . $projectId . "` (`interview_info_id`, `project_id`, `respondent_id`, `q_id`, `response`, `responded_at`, `q_elapsed_time`, `q_order`, `resp_order`, `created_at`, `deleted_at`)
                       VALUES (" . $AutoId . "," . $projectId . "," . $RespondentId . ",'" . $arrayQId[$i] . "','" . $arrayResponse[$i] . "','" . date('Y-m-d H:i:s', strtotime($arrayResponseDateTime[$i])) . "','" . $arrayqElapsedTime[$i] . "'," . $arrayqOrderTag[$i] . "," . $arrayrOrderTag[$i] . ",'" . $DateOfSync . "', NULL)";
                if ($conn->query($sql) !== TRUE) {
                    echo "Error: " . $sql . "<br>" . $conn->error;
                }
            }
            //************ Insert into T_Openended *******************
            if($arrayQIdOE!=null)
            {
                if (sizeof($arrayQIdOE) >0 && $arrayQIdOE[0]!='') {
                    for ($i = 0; $i < sizeof($arrayQIdOE); $i++) {
                        $sql = "INSERT INTO `open_endeds_" . $projectId . "` (`interview_info_id`, `project_id`, `respondent_id`, `q_id`, `attribute_value`, `response`, `response_type`, `created_at`, `deleted_at`)
                           VALUES (" . $AutoId . "," . $projectId . "," . $RespondentId . ",'" . $arrayQIdOE[$i] . "','" . $arrayAttributeValue[$i] . "','" . $arrayOpenendedResp[$i] . "','" . $arrayTypeOfOE[$i] . "','" . $DateOfSync . "', NULL)";
                        if ($conn->query($sql) !== TRUE) {
                            echo "Error: " . $sql . "<br>" . $conn->error;
                        }
                    }
                }
            }
            echo "New record created successfully";
        } else {
            echo "Error: " . $sql . "<br>" . $conn->error;
        }
        
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