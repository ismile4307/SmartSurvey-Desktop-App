<?php
$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_surveyhive";



// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
mysqli_query($conn,"SET character_set_results = 'utf8', character_set_client = 'utf8', character_set_connection = 'utf8', character_set_database = 'utf8', character_set_server 		= 'utf8'");
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
// if (isset($_POST['ProjectId'])) {
//     $projectId = $_POST['ProjectId'];
// }
// if (isset($_POST['RespondentId'])) {
//     $RespondentId = $_POST['RespondentId'];
// }
// if (isset($_POST['Latitude'])) {
//     $Latitude= $_POST['Latitude'];
// }
// if (isset($_POST['Longitude'])) {
//     $Longitude= $_POST['Longitude'];
// }
// if (isset($_POST['SurveyDateTime'])) {
//     $SurveyDateTime = date('Y-m-d H:i:s', strtotime($_POST['SurveyDateTime']));
// }
// if (isset($_POST['SurveyEndTime'])) {
//     $SurveyEndTime = date('Y-m-d H:i:s', strtotime($_POST['SurveyEndTime']));
// }
// if (isset($_POST['LengthOfIntv'])) {
//     $LengthOfIntv = $_POST['LengthOfIntv'];
// }
// if (isset($_POST['Intv_Type'])) {
//     $Intv_Type = $_POST['Intv_Type'];
// }
// if (isset($_POST['FICode'])) {
//     $FICode = $_POST['FICode'];
// }
// if (isset($_POST['FSCode'])) {
//     $FSCode = $_POST['FSCode'];
// }
// if (isset($_POST['AccompaniedBy'])) {
//     $AccompaniedBy = $_POST['AccompaniedBy'];
// }
// if (isset($_POST['BackCheckedBy'])) {
//     $BackCheckedBy = $_POST['BackCheckedBy'];
// }
// if (isset($_POST['Status'])) {
//     $Status = $_POST['Status'];
// }
// if (isset($_POST['TabId'])) {
//     $TabId = $_POST['TabId'];
// }
// if (isset($_POST['SyncStatus'])) {
//     $SyncStatus = $_POST['SyncStatus'];
// }
// if (isset($_POST['ScriptVersion'])) {
//     $ScriptVersion = $_POST['ScriptVersion'];
// }
// if (isset($_POST['LanguageId'])) {
//     $LanguageId = $_POST['LanguageId'];
// }
// if (isset($_POST['FieldExtra1'])) {
//     $FieldExtra1 = $_POST['FieldExtra1'];
// }
// if (isset($_POST['FieldExtra2'])) {
//     $FieldExtra2 = $_POST['FieldExtra2'];
// }
// if (isset($_POST['SyncDateTime'])) {
//     $DateOfSync = $_POST['SyncDateTime'];
// }


if (isset($_POST['project_id'])) {
    $arrayproject_id = $_POST['project_id'];
}
if (isset($_POST['qid'])) {
    $arrayqid = $_POST['qid'];
}
if (isset($_POST['question_english'])) {
    $arrayquestion_english = $_POST['question_english'];
}
if (isset($_POST['question_bengali'])) {
    $arrayquestion_bengali = $_POST['question_bengali'];//date('Y-m-d H:i:s', strtotime($_POST['ResponseDateTime']));

}
if (isset($_POST['attribute_id'])) {
    $arrayqattribute_id = $_POST['attribute_id'];
}
if (isset($_POST['comments'])) {
    $arrayqcomments = $_POST['comments'];
}
if (isset($_POST['qtype'])) {
    $arrayrqtype = $_POST['qtype'];
}
if (isset($_POST['no_of_response_min'])) {
    $arrayno_of_response_min = $_POST['no_of_response_min'];
}
if (isset($_POST['no_of_response_max'])) {
    $arrayno_of_response_max = $_POST['no_of_response_max'];
}
if (isset($_POST['has_auto_response'])) {
    $arrayhas_auto_response = $_POST['has_auto_response'];
}
if (isset($_POST['has_random_attrib'])) {
    $arrayhas_random_attrib = $_POST['has_random_attrib'];
}

if (isset($_POST['number_of_column'])) {
    $arraynumber_of_column = $_POST['number_of_column'];
}
if (isset($_POST['show_in_report'])) {
    $arrayshow_in_report = $_POST['show_in_report'];
}
if (isset($_POST['has_random_qntr'])) {
    $arrayhas_random_qntr = $_POST['has_random_qntr'];
}
if (isset($_POST['has_message_logic'])) {
    $arrayhas_message_logic = $_POST['has_message_logic'];
}
if (isset($_POST['written_oe_in_paper'])) {
    $arraywritten_oe_in_paper = $_POST['written_oe_in_paper'];
}
if (isset($_POST['force_to_take_oe'])) {
    $arrayforce_to_take_oe = $_POST['force_to_take_oe'];
}
if (isset($_POST['has_media_path'])) {
    $arrayhas_media_path = $_POST['has_media_path'];
}
if (isset($_POST['display_back_button'])) {
    $arraydisplay_back_button = $_POST['display_back_button'];
}
if (isset($_POST['display_next_button'])) {
    $arraydisplay_next_button = $_POST['display_next_button'];
}
if (isset($_POST['display_jump_button'])) {
    $arraydisplay_jump_button = $_POST['display_jump_button'];
}
if (isset($_POST['resume_qntr_jump'])) {
    $arrayresume_qntr_jump = $_POST['resume_qntr_jump'];
}
if (isset($_POST['silent_recording'])) {
    $arraysilent_recording = $_POST['silent_recording'];
}
if (isset($_POST['file_path'])) {
    $arrayfile_path = $_POST['file_path'];
}
if (isset($_POST['order_tag'])) {
    $arrayorder_tag = $_POST['order_tag'];
}
if (isset($_POST['order_tag1'])) {
    $arrayorder_tag1 = $_POST['order_tag1'];
}
if (isset($_POST['order_tag2'])) {
    $arrayorder_tag2 = $_POST['order_tag2'];
}
if (isset($_POST['order_tag3'])) {
    $arrayorder_tag3 = $_POST['order_tag3'];
}
if (isset($_POST['order_tag4'])) {
    $arrayorder_tag4 = $_POST['order_tag4'];
}
if (isset($_POST['order_tag5'])) {
    $arrayorder_tag5 = $_POST['order_tag5'];
}
if (isset($_POST['question_lang3'])) {
    $arrayquestion_lang3 = $_POST['question_lang3'];
}
if (isset($_POST['question_lang4'])) {
    $arrayquestion_lang4 = $_POST['question_lang4'];
}
if (isset($_POST['question_lang5'])) {
    $arrayquestion_lang5 = $_POST['question_lang5'];
}
if (isset($_POST['question_lang6'])) {
    $arrayquestion_lang6 = $_POST['question_lang6'];
}
if (isset($_POST['question_lang7'])) {
    $arrayquestion_lang7 = $_POST['question_lang7'];
}
if (isset($_POST['question_lang8'])) {
    $arrayquestion_lang8 = $_POST['question_lang8'];
}
if (isset($_POST['question_lang9'])) {
    $arrayquestion_lang9 = $_POST['question_lang9'];
}
if (isset($_POST['question_lang10'])) {
    $arrayquestion_lang10 = $_POST['question_lang10'];
}
if (isset($_POST['listCounter'])) {
    $listCounter = $_POST['listCounter'];
}



// echo $arrayorder_tag5;

//********* Delete prior all record

if($listCounter==1){
    $query="DELETE FROM questions WHERE project_id=".$arrayproject_id[0];
    
    // echo $query;
    $conn->query($query);
}
//***************************


for ($i = 0; $i < sizeof($arrayproject_id); $i++) {
    //var_dump($arrayResponseDateTime);
    //debug_to_console($arrayResponseDateTime);
    $sql = "INSERT INTO `questions`(`project_id`, `qid`, `question_english`, `question_bengali`, `attribute_id`, `comments`, `qtype`, `no_of_response_min`, `no_of_response_max`, `has_auto_response`, `has_random_attrib`, `number_of_column`, `show_in_report`, `has_random_qntr`, `has_message_logic`, `written_oe_in_paper`, `force_to_take_oe`, `has_media_path`, `display_back_button`, `display_next_button`, `display_jump_button`, `resume_qntr_jump`, `silent_recording`, `file_path`, `order_tag`, `order_tag1`, `order_tag2`, `order_tag3`, `order_tag4`, `order_tag5`, `question_lang3`, `question_lang4`, `question_lang5`, `question_lang6`, `question_lang7`, `question_lang8`, `question_lang9`, `question_lang10`) 
            VALUES (".$arrayproject_id[$i].",'".$arrayqid[$i]."','".$arrayquestion_english[$i]."','".$arrayquestion_bengali[$i]."','".$arrayqattribute_id[$i]."','".$arrayqcomments[$i]."','".$arrayrqtype[$i]."','".$arrayno_of_response_min[$i]."','".$arrayno_of_response_max[$i]."','".$arrayhas_auto_response[$i]."','".$arrayhas_random_attrib[$i]."','".$arraynumber_of_column[$i]."','".$arrayshow_in_report[$i]."','".$arrayhas_random_qntr[$i]."','".$arrayhas_message_logic[$i]."','".$arraywritten_oe_in_paper[$i]."','".$arrayforce_to_take_oe[$i]."','".$arrayhas_media_path[$i]."','".$arraydisplay_back_button[$i]."','".$arraydisplay_next_button[$i]."','".$arraydisplay_jump_button[$i]."','".$arrayresume_qntr_jump[$i]."','".$arraysilent_recording[$i]."','".$arrayfile_path[$i]."',".$arrayorder_tag[$i].",".$arrayorder_tag1[$i].",".$arrayorder_tag2[$i].",".$arrayorder_tag3[$i].",".$arrayorder_tag4[$i].",".$arrayorder_tag5[$i].",'".$arrayquestion_lang3[$i]."','".$arrayquestion_lang4[$i]."','".$arrayquestion_lang5[$i]."','".$arrayquestion_lang6[$i]."','".$arrayquestion_lang7[$i]."','".$arrayquestion_lang8[$i]."','".$arrayquestion_lang9[$i]."','".$arrayquestion_lang10[$i]."')";
    if ($conn->query($sql) === TRUE) {
        //echo "New record created successfully";
        //echo json_encode(array('message'=>"New record created successfully"));
    } else {
        echo "Error: " . $sql . "<br>" . $conn->error;
    }
}


echo "New record created successfully";




// $AutoId=0;

// $recExist = "SELECT * FROM interview_infos WHERE respondent_id=" . $RespondentId;

// if (mysqli_query($conn, $recExist)) {
//     $noOfRecord = mysqli_num_rows(mysqli_query($conn, $recExist));
//     //echo $noOfRecord;
//     if ($noOfRecord > 0) {
//         //$update = mysqli_query($conn, "UPDATE interview_infos SET deleted_at='" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "' WHERE respondent_id=" . $RespondentId );

//         //if ($update) {
//         //    //throw new Exception("Update successful");
//         //    //echo "Update successful";
//         //} else {
//         //    //echo "Error: " . $update . "<br>" . $conn->error;
//         //    echo "Update error";
//         //}
//         echo "Record already exists";
//     } else {
//         //Insert
//         //echo "insert success";
        
//         //*****************************************************************************


// //************ Insert into T_InterviewInfo *******************
// $sql = "INSERT INTO `interview_infos`(`project_id`, `respondent_id`, `latitude`, `longitude`, `survey_start_at`, `survey_end_at`, `length_of_intv`, `intv_type`, `fi_code`, `fs_code`, `accompanied_by`, `back_checked_by`, `status`, `tab_id`, `sync_status`, `script_version`, `language_id`, `field_ex1`, `field_ex2`, `created_at`, `deleted_at`)
// VALUES (" . $projectId . "," . $RespondentId . ",'" . $Latitude . "','" . $Longitude . "','" . $SurveyDateTime . "','" . $SurveyEndTime . "','" . $LengthOfIntv . "','" . $Intv_Type . "','" . $FICode . "','" . $FSCode . "','" . $AccompaniedBy . "','" . $BackCheckedBy . "','" . $Status . "','" . $TabId . "','" . $SyncStatus . "','" . $ScriptVersion . "','" . $LanguageId . "','" . $FieldExtra1 . "','" . $FieldExtra2 . "','" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "', NULL)";

// if ($conn->query($sql) === TRUE) {
//     //echo "New record created successfully";
//     //echo json_encode(array('message'=>"New record created successfully"));
//     $AutoId=mysqli_insert_id($conn);
// } else {
//     echo "Error: " . $sql . "<br>" . $conn->error;
// }
//************ Insert into T_RespAnswer *******************
// for ($i = 0; $i < sizeof($arrayQId); $i++) {
//     //var_dump($arrayResponseDateTime);
//     //debug_to_console($arrayResponseDateTime);
//     $sql = "INSERT INTO `answers`(`interview_info_id`, `project_id`, `respondent_id`, `q_id`, `response`, `responded_at`, `q_elapsed_time`, `q_order`, `resp_order`, `created_at`, `deleted_at`)
//           VALUES (" . $AutoId . "," . $projectId . "," . $RespondentId . ",'" . $arrayQId[$i] . "','" . $arrayResponse[$i] . "','" . date('Y-m-d H:i:s', strtotime($arrayResponseDateTime[$i])) . "','" . $arrayqElapsedTime[$i] . "'," . $arrayqOrderTag[$i] . "," . $arrayrOrderTag[$i] . ",'" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "','')";
//     if ($conn->query($sql) === TRUE) {
//         //echo "New record created successfully";
//         //echo json_encode(array('message'=>"New record created successfully"));
//     } else {
//         echo "Error: " . $sql . "<br>" . $conn->error;
//     }
// }
//************ Insert into T_Openended *******************
// if (sizeof($arrayQIdOE) >0 && $arrayQIdOE[0]!='') {
//     //echo sizeof($arrayQIdOE);
//     for ($i = 0; $i < sizeof($arrayQIdOE); $i++) {
//         //var_dump($arrayResponseDateTime);
//         //debug_to_console($arrayResponseDateTime);
//         $sql = "INSERT INTO `open_endeds`(`interview_info_id`, `project_id`, `respondent_id`, `q_id`, `attribute_value`, `response`, `response_type`, `created_at`, `deleted_at`)
//           VALUES (" . $AutoId . "," . $projectId . "," . $RespondentId . ",'" . $arrayQIdOE[$i] . "','" . $arrayAttributeValue[$i] . "','" . $arrayOpenendedResp[$i] . "','" . $arrayTypeOfOE[$i] . "','" . date('Y-m-d H:i:s', strtotime($DateOfSync)) . "','')";
//         if ($conn->query($sql) === TRUE) {
//             //echo "New record created successfully";
//             //echo json_encode(array('message'=>"New record created successfully"));
//         } else {
//             echo "Error: " . $sql . "<br>" . $conn->error;
//         }
//     }
// }
//if ($conn->query($sql) === TRUE) {
// echo "New record created successfully";
//echo json_encode(array('message'=>"New record created successfully"));
//} else {
//    echo "Error: " . $sql . "<br>" . $conn->error;
//}

//**********************************************************


//     }
// } else {
//     echo "Error: " . $recExist . "<br>" . $conn->error;
//     //echo "Rec exist error";
// }




$conn->close();
/*$projectId = $_REQUEST['ProjectId'];
for($i=0; $i< count($projectId); $i++){
    echo $projectId[$i];
}
*/
?>