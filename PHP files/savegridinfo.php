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
if (isset($_POST['qid'])) {
    $arrayqid = $_POST['qid'];
}
if (isset($_POST['attribute_english'])) {
    $arrayattribute_english = $_POST['attribute_english'];
}
if (isset($_POST['attribute_bengali'])) {
    $arrayattribute_bengali = $_POST['attribute_bengali'];//date('Y-m-d H:i:s', strtotime($_POST['ResponseDateTime']));

}
if (isset($_POST['attribute_value'])) {
    $arrayqattribute_value = $_POST['attribute_value'];
}
if (isset($_POST['attribute_order'])) {
    $arrayqattribute_order = $_POST['attribute_order'];
}
if (isset($_POST['take_openended'])) {
    $arrayrtake_openended = $_POST['take_openended'];
}
if (isset($_POST['is_exclusive'])) {
    $arrayis_exclusive = $_POST['is_exclusive'];
}
if (isset($_POST['min_value'])) {
    $arraymin_value = $_POST['min_value'];
}

if (isset($_POST['max_value'])) {
    $arraymax_value = $_POST['max_value'];
}
if (isset($_POST['force_and_msg_opt'])) {
    $arrayforce_and_msg_opt = $_POST['force_and_msg_opt'];
}
if (isset($_POST['comments'])) {
    $arraycomments = $_POST['comments'];
}
if (isset($_POST['attribute_lang3'])) {
    $arrayattribute_lang3 = $_POST['attribute_lang3'];
}
if (isset($_POST['attribute_lang4'])) {
    $arrayattribute_lang4 = $_POST['attribute_lang4'];
}
if (isset($_POST['attribute_lang5'])) {
    $arrayattribute_lang5 = $_POST['attribute_lang5'];
}
if (isset($_POST['attribute_lang6'])) {
    $arrayattribute_lang6 = $_POST['attribute_lang6'];
}
if (isset($_POST['attribute_lang7'])) {
    $arrayattribute_lang7 = $_POST['attribute_lang7'];
}
if (isset($_POST['attribute_lang8'])) {
    $arrayattribute_lang8 = $_POST['attribute_lang8'];
}
if (isset($_POST['attribute_lang9'])) {
    $arrayattribute_lang9 = $_POST['attribute_lang9'];
}
if (isset($_POST['attribute_lang10'])) {
    $arrayattribute_lang10 = $_POST['attribute_lang10'];
}
if (isset($_POST['listCounter'])) {
    $listCounter = $_POST['listCounter'];
}



// echo $arrayorder_tag5;

//********* Delete prior all record

if($listCounter==1){
    $query="DELETE FROM grid_infos WHERE project_id=".$arrayproject_id[0];
    
    // echo $query;
    $conn->query($query);
}
//***************************


for ($i = 0; $i < sizeof($arrayproject_id); $i++) {
    //var_dump($arrayResponseDateTime);
    //debug_to_console($arrayResponseDateTime);
    $sql = "INSERT INTO `grid_infos`(`project_id`, `qid`, `attribute_english`, `attribute_bengali`, `attribute_value`, `attribute_order`, `take_openended`, `is_exclusive`,`min_value`, `max_value`, `force_and_msg_opt`,`comments`, `attribute_lang3`, `attribute_lang4`, `attribute_lang5`, `attribute_lang6`, `attribute_lang7`, `attribute_lang8`, `attribute_lang9`, `attribute_lang10`) 
            VALUES (".$arrayproject_id[$i].",'".$arrayqid[$i]."','".$arrayattribute_english[$i]."','".$arrayattribute_bengali[$i]."','".$arrayqattribute_value[$i]."',".$arrayqattribute_order[$i].",'".$arrayrtake_openended[$i]."','".$arrayis_exclusive[$i]."','".$arraymin_value[$i]."','".$arraymax_value[$i]."','".$arrayforce_and_msg_opt[$i]."','".$arraycomments[$i]."','".$arrayattribute_lang3[$i]."','".$arrayattribute_lang4[$i]."','".$arrayattribute_lang5[$i]."','".$arrayattribute_lang6[$i]."','".$arrayattribute_lang7[$i]."','".$arrayattribute_lang8[$i]."','".$arrayattribute_lang9[$i]."','".$arrayattribute_lang10[$i]."');";
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