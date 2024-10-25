<?php

//echo json_encode($_POST);

//echo json_encode($_GET);


include("include/db_connect.php");

//$tInterviewInfoId = DB_Util::insert('test_syncing', ['data' => json_encode($_POST)] );

$saveData = array();
if (isset($_POST['RespondentId'])) {
$saveData['respondent_id'] = $_POST['RespondentId'];
}
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


if(isset($_FILES['audio1']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio1']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio1']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio2']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio2']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio2']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio3']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio3']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio3']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio4']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio4']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio4']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio5']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio5']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio5']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio6']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio6']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio6']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio7']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio7']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio7']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio8']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio8']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio8']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio9']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio9']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio9']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio10']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio10']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio10']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio11']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio11']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio11']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio12']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio12']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio12']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio13']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio13']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio13']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio14']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio14']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio14']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio15']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio15']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio15']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio16']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio16']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio16']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio17']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio17']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio17']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio18']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio18']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio18']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['audio19']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio19']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio19']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}


if(isset($_FILES['audio20']['name'])) {
    $uploaddir = "audio/";
    $file = basename($_FILES['audio20']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['audio20']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

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

if(isset($_FILES['image6']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image6']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image6']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image7']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image7']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image7']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image8']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image8']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image8']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image9']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image9']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image9']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image10']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image10']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image10']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image11']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image11']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image11']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image12']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image12']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image12']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image13']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image13']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image13']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image14']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image14']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image14']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image15']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image15']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image15']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image16']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image16']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image16']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image17']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image17']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image17']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image18']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image18']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image18']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image19']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image19']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image19']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image20']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image20']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image20']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image21']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image21']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image21']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image22']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image22']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image22']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image23']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image23']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image23']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image24']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image24']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image24']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

if(isset($_FILES['image25']['name'])) {
    $uploaddir = "images/";
    $file = basename($_FILES['image25']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image25']['tmp_name'], $uploadfile)) {
      //  $saveData['IMAGE_URL'] = $uploadfile;
    }
}

$myProjectId=$saveData['project_id'];

if(isset($saveData))
{
DB_Util::softDeleteIfExist('interview_infos_'.$myProjectId, $saveData);
$tInterviewInfoId = DB_Util::insert('interview_infos_'.$myProjectId, $saveData);
}

if (!$tInterviewInfoId) {
    renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
   // renderJSON(array('success' => false, 'message' => 'Could not save data'));
}


if (isset($_POST['Response'])) {
    //  echo $_POST['Response'];
    if(isset($_POST['Response']))
    {
        $response = json_decode($_POST['Response']);
    }
    
    // if(isset($saveData))
    // {
    //     DB_Util::softDeleteIfExist('answers', $saveData);
    // }
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
            $RespSaveData['response'] = str_replace('\'', '', $resp->{'Response'});
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
        $res = DB_Util::insert('answers_'.$myProjectId, $RespSaveData);
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
        $res = DB_Util::insert('open_endeds_'.$myProjectId, $RespSaveData);
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