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


if (isset($_POST['project_id'])) {
    $arrayproject_id = $_POST['project_id'];
}
if (isset($_POST['language_id'])) {
    $arraylanguage_id = $_POST['language_id'];
}
if (isset($_POST['language_name'])) {
    $arraylanguage_name = $_POST['language_name'];
}
if (isset($_POST['status'])) {
    $arraystatus = $_POST['status'];//date('Y-m-d H:i:s', strtotime($_POST['ResponseDateTime']));
}
if (isset($_POST['display_order'])) {
    $arraydisplay_order = $_POST['display_order'];
}
if (isset($_POST['listCounter'])) {
    $listCounter = $_POST['listCounter'];
}



// echo $arrayorder_tag5;

//********* Delete prior all record

if($listCounter==1){
    $query="DELETE FROM language_masters WHERE project_id=".$arrayproject_id[0];
    
    // echo $query;
    $conn->query($query);
}
//***************************


for ($i = 0; $i < sizeof($arrayproject_id); $i++) {
    //var_dump($arrayResponseDateTime);
    //debug_to_console($arrayResponseDateTime);
    $sql = "INSERT INTO `language_masters`(`project_id`, `language_id`, `language_name`, `status`, `display_order`) 
            VALUES (".$arrayproject_id[$i].",".$arraylanguage_id[$i].",'".$arraylanguage_name[$i]."',".$arraystatus[$i].",".$arraydisplay_order[$i].");";
    if ($conn->query($sql) === TRUE) {
        //echo "New record created successfully";
        //echo json_encode(array('message'=>"New record created successfully"));
    } else {
        echo "Error: " . $sql . "<br>" . $conn->error;
    }
}


echo "New record created successfully";

$conn->close();

?>