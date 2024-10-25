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

$saveData['created_at'] = date('Y-m-d H:i:s');


if(isset($saveData))
{
if (DB_Util::softDeleteIfExist('interview_infos', $saveData))
	renderJSON(array('success' => true, 'message' => 'Data has saved successfully'));
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