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
if (isset($_POST['logic_id'])) {
    $arraylogic_id = $_POST['logic_id'];
}
if (isset($_POST['qid'])) {
    $arrayqid = $_POST['qid'];
}
if (isset($_POST['logic_type_id'])) {
    $arraylogic_type_id = $_POST['logic_type_id'];//date('Y-m-d H:i:s', strtotime($_POST['ResponseDateTime']));
}
if (isset($_POST['if_condition'])) {
    $arrayif_condition = $_POST['if_condition'];
}
if (isset($_POST['then_value'])) {
    $arraythen_value = $_POST['then_value'];
}
if (isset($_POST['else_value'])) {
    $arrayelse_value = $_POST['else_value'];
}    
if (isset($_POST['listCounter'])) {
    $listCounter = $_POST['listCounter'];
}



// echo $arrayorder_tag5;

//********* Delete prior all record

if($listCounter==1){
    $query="DELETE FROM logic_tables WHERE project_id=".$arrayproject_id[0];
    
    // echo $query;
    $conn->query($query);
}
//***************************


for ($i = 0; $i < sizeof($arrayproject_id); $i++) {
    //var_dump($arrayResponseDateTime);
    //debug_to_console($arrayResponseDateTime);
    $sql = "INSERT INTO `logic_tables`(`project_id`, `logic_id`, `qid`, `logic_type_id`, `if_condition`, `then_value`, `else_value`) 
            VALUES (".$arrayproject_id[$i].",".$arraylogic_id[$i].",'".$arrayqid[$i]."','".$arraylogic_type_id[$i]."','".$arrayif_condition[$i]."','".$arraythen_value[$i]."','".$arrayelse_value[$i]."');";
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