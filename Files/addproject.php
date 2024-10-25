<?php
$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_capi";



// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
if (isset($_POST['projectName'])) {
    $projectName = $_POST['projectName'];
}
if (isset($_POST['projectCode'])) {
    $projectCode = $_POST['projectCode'];
}
if (isset($_POST['databaseName'])) {
    $databaseName= $_POST['databaseName'];
}
if (isset($_POST['startDate'])) {
    $startDate= $_POST['startDate'];
}
if (isset($_POST['projectStatus'])) {
    $projectStatus = $_POST['projectStatus'];
}

$AutoId=0;

$recExist = "SELECT * FROM project_infos WHERE project_code='" . $projectCode . "'";

if (mysqli_query($conn, $recExist)) {
    $noOfRecord = mysqli_num_rows(mysqli_query($conn, $recExist));
    //echo $noOfRecord;
    if ($noOfRecord > 0) {
        echo "Record already exists";
    } else {
		//************ Insert into T_InterviewInfo *******************
		$sql = "INSERT INTO `project_infos`(`project_name`, `project_code`, `database_name`, `start_date`, `status`, `deleted_at`)
		VALUES ('" . $projectName . "','" . $projectCode . "','" . $databaseName . "','" . $startDate . "','" . $projectStatus."', NULL)";

		if ($conn->query($sql) === TRUE) {
			//echo "New record created successfully";
			//echo json_encode(array('message'=>"New record created successfully"));
			$AutoId=mysqli_insert_id($conn);
		} else {
			echo "Error: " . $sql . "<br>" . $conn->error;
		}
	echo "New record created successfully";
	}
} else {
    echo "Error: " . $recExist . "<br>" . $conn->error;
}

$conn->close();
?>