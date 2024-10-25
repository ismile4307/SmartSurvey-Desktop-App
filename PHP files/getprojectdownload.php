<?php
$servername = "localhost";
$username = "survfiqz_ismile";
$password = "Arnisha@4307#";
$dbname = "survfiqz_capi";



// Create connection
$connection = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($connection->connect_error) {
    die("Connection failed: " . $connection->connect_error);
}


   $query = "SELECT `id`, `project_name`, `project_code`, `database_name`, `start_date`, `status` FROM `project_infos` WHERE status='Active'";
  

   $result = mysqli_query($connection, $query) or die("Error in Selecting " . mysqli_error($connection));

    //create an array
    $emparray = array();
    while($row =mysqli_fetch_assoc($result))
    {
        $emparray[] = $row;
    }
    echo json_encode($emparray);

    //close the db connection
    mysqli_close($connection);
?>