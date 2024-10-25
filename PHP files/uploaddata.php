<?php
//include("include/db_connect.php");


if(isset($_FILES['image1']['name'])) {
    $uploaddir = "ansdb/";
    $file = basename($_FILES['image1']['name']);
    $uploadfile = $uploaddir . $file;
    if(file_exists($uploadfile)) {
        unlink($uploadfile);
    }
    if (move_uploaded_file($_FILES['image1']['tmp_name'], $uploadfile)) {
        $saveData['IMAGE_URL'] = $uploadfile;
    }
}


renderJSON(array('success' => true, 'message' => 'Data has saved successfully'));

function renderJSON($data = array())
{
    header('Content-Type: application/json');
    echo json_encode($data);
    exit();
}