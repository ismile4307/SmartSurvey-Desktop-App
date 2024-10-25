<?php

$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_surveyhive";



// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
if (isset($_POST['projectId'])) {
    $projectID = $_POST['projectId'];
}
if (isset($_POST['scriptVersion'])) {
    $scriptVersion = $_POST['scriptVersion'];
}


// $sql = "UPDATE MyGuests SET lastname='Doe' WHERE id=2";
$sql = "UPDATE `projects` SET `script_version`='".$scriptVersion."' WHERE `project_code`=".$projectID;

if ($conn->query($sql) === TRUE) {
  echo "Record updated successfully";
} else {
  echo "Error updating record: " . $conn->error;
}

		
$conn->close();
?>

