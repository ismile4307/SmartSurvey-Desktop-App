<?php
$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_capiapp";



// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
if (isset($_POST['projectName'])) {
    $projectName = $_POST['projectName'];
}
if (isset($_POST['projectId'])) {
    $projectId = $_POST['projectId'];
}
if (isset($_POST['databaseName'])) {
    $databaseName= $_POST['databaseName'];
}
if (isset($_POST['scriptVersion'])) {
    $scriptVersion= $_POST['scriptVersion'];
}
if (isset($_POST['mediaVersion'])) {
    $mediaVersion= $_POST['mediaVersion'];
}
if (isset($_POST['startDate'])) {
    $startDate= $_POST['startDate'];
}
if (isset($_POST['projectStatus'])) {
    $projectStatus = $_POST['projectStatus'];
}
if (isset($_POST['typeOfOperation'])) {
    $typeOfOperation = $_POST['typeOfOperation'];
}

if($typeOfOperation==1){
    $AutoId=0;
    
    $recExist = "SELECT * FROM projects WHERE project_code='" . $projectCode . "'";
    
    if (mysqli_query($conn, $recExist)) {
        $noOfRecord = mysqli_num_rows(mysqli_query($conn, $recExist));
        //echo $noOfRecord;
        if ($noOfRecord > 0) {
            echo "Record already exists with same project code";
        } else {
    		//************ Insert into T_InterviewInfo *******************
    		$sql = "INSERT INTO `projects`(`project_code`, `project_name`, `script_version`, `media_version`, `client_name`, `start_date`, `database_name`, `status`, `deleted_at`) 
    		VALUES ('" . $projectId . "','" . $projectName . "','" . $scriptVersion . "','" . $mediaVersion . "','','" . $startDate . "','" . $databaseName . "'," . $projectStatus . ",NULL)";
    
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
}
else if($typeOfOperatoin==2){
    
}

$conn->close();
?>