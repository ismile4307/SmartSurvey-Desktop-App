<?php
include("include/db_connect.php");

$saveData = array();
$saveData['respondent_id'] = $_POST['RespondentId'];
if (isset($_POST['ProjectId'])) {
    $saveData['project_id'] = $_POST['ProjectId'];
}
if (isset($_POST['Latitude'])) {
    $saveData['latitude'] = $_POST['Latitude'];
}
if (isset($_POST['Longitude'])) {
    $saveData['longitude'] = $_POST['Longitude'];
}
if (isset($_POST['SurveyDateTime'])) {
    $saveData['survey_start_at'] = $_POST['SurveyDateTime'];
}
if (isset($_POST['SurveyEndTime'])) {
    $saveData['survey_end_at'] = $_POST['SurveyEndTime'];
}
if (isset($_POST['LengthOfIntv'])) {
    $saveData['length_of_intv'] = $_POST['LengthOfIntv'];
}
if (isset($_POST['Intv_Type'])) {
    $saveData['intv_type'] = $_POST['Intv_Type'];
}
if (isset($_POST['FICode'])) {
    $saveData['fi_code'] = $_POST['FICode'];
}
if (isset($_POST['FSCode'])) {
    $saveData['fs_code'] = $_POST['FSCode'];
}
if (isset($_POST['AccompaniedBy'])) {
    $saveData['accompanied_by'] = $_POST['AccompaniedBy'];
}
if (isset($_POST['BackCheckedBy'])) {
    $saveData['back_checked_by'] = $_POST['BackCheckedBy'];
}
if (isset($_POST['Status'])) {
    $saveData['status'] = $_POST['Status'];
}
if (isset($_POST['TabId'])) {
    $saveData['tab_id'] = $_POST['TabId'];
}
if (isset($_POST['SyncStatus'])) {
    $saveData['sync_status'] = $_POST['SyncStatus'];
}
if (isset($_POST['ScriptVersion'])) {
    $saveData['script_version'] = $_POST['ScriptVersion'];
}
if (isset($_POST['LanguageId'])) {
    $saveData['language_id'] = $_POST['LanguageId'];
}
if (isset($_POST['FieldExtra1'])) {
    $saveData['field_ex1'] = $_POST['FieldExtra1'];
}
if (isset($_POST['FieldExtra2'])) {
    $saveData['field_ex2'] = $_POST['FieldExtra2'];
}

$saveData['created_at'] = date('Y-m-d H:i:s');


if(isset($_FILES['image1']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image1']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image1']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image2']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image2']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image2']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image3']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image3']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image3']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image4']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image4']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image4']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image5']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image5']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image5']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

DB_Util::softDeleteIfExist('interview_infos', $saveData);
$tInterviewInfoId = DB_Util::insert('interview_infos', $saveData);

if (!$tInterviewInfoId) {
	renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
   // renderJSON(array('success' => false, 'message' => 'Could not save data'));
}


if (isset($_POST['Response'])) {
    //  echo $_POST['Response'];
    $response = json_decode($_POST['Response']);
    
    DB_Util::softDeleteIfExist('answers', $saveData);

    //echo $response;
    foreach ($response->{'Response'} as $resp) {
        $RespSaveData = array();
        $RespSaveData['interview_info_id'] = $tInterviewInfoId;
        $RespSaveData['project_id'] = $saveData['project_id'];
        $RespSaveData['respondent_id'] = $saveData['respondent_id'];
        if (isset($resp->{'QId'})) {
            $RespSaveData['q_id'] = $resp->{'QId'};
        }
        if (isset($resp->{'Response'})) {
            $RespSaveData['response'] = $resp->{'Response'};
        }
        if (isset($resp->{'ResponseDateTime'})) {
            $RespSaveData['responded_at'] = $resp->{'ResponseDateTime'};
        }
        if (isset($resp->{'qElapsedTime'})) {
            $RespSaveData['q_elapsed_time'] = $resp->{'qElapsedTime'};
        }
        if (isset($resp->{'qOrderTag'})) {
            $RespSaveData['q_order'] = $resp->{'qOrderTag'};
        }
        if (isset($resp->{'rOrderTag'})) {
            $RespSaveData['resp_order'] = $resp->{'rOrderTag'};
        }
        $RespSaveData['created_at'] = date('Y-m-d H:i:s');
        $res = DB_Util::insert('answers', $RespSaveData);
        if (!$res) {
	  renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
	}
    }
}

if (isset($_POST['OpenEndedResponse'])) {
    //  echo $_POST['Response'];
    $response = json_decode($_POST['OpenEndedResponse']);

    //echo $response;
    foreach ($response->{'OpenEndedResponse'} as $resp) {
        $RespSaveData = array();
        $RespSaveData['interview_info_id'] = $tInterviewInfoId;
        $RespSaveData['project_id'] = $saveData['project_id'];
        $RespSaveData['respondent_id'] = $saveData['respondent_id'];
        if (isset($resp->{'QId'})) {
            $RespSaveData['q_id'] = $resp->{'QId'};
        }
        if (isset($resp->{'AttributeValue'})) {
            $RespSaveData['attribute_value'] = $resp->{'AttributeValue'};
        }
        if (isset($resp->{'OpenendedResp'})) {
            $RespSaveData['response'] = $resp->{'OpenendedResp'};
        }
        if (isset($resp->{'OEResponseType'})) {
            $RespSaveData['response_type'] = $resp->{'OEResponseType'};
        }
        $RespSaveData['created_at'] = date('Y-m-d H:i:s');
        $res = DB_Util::insert('open_endeds', $RespSaveData);
        if (!$res) {
            renderJSON(array('success' => false, 'message' => 'Could not save openended data'));
        }
    }
}


renderJSON(array('success' => true, 'message' => 'Data has saved successfully'));

function renderJSON($data = array())
{
    header('Content-Type: application/json');
    echo json_encode($data);
    exit();
}