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
	$projectCode= $_POST['ProjectId'];
}

$saveData['created_at'] = date('Y-m-d H:i:s');


if(isset($saveData))
{
$res=DB_Util::softDeleteIfExist('interview_infos_'.$projectCode, $saveData);
if (!$res) {
            renderJSON(array('success' => true, 'message' => $saveData['respondent_id'].' is rejected successfully'));
        }
//	renderJSON(array('success' => true, 'message' => 'Data has saved successfully'));
//echo "Data has saved successfully";
}

//if (!$tInterviewInfoId) {
//    renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
//   // renderJSON(array('success' => false, 'message' => 'Could not save data'));
//}


function renderJSON($data = array())
{
    header('Content-Type: application/json');
    echo json_encode($data);
    exit();
}