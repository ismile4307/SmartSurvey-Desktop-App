<?php

//echo 'Current PHP version: ' . phpversion();

	$dbnamex=null;


if (isset($_POST["dbname"]))
{
$dbnamex = $_POST['dbname'];
}

 //$ficode = $_POST['FICode'];


//echo $dbnamex;

class MyDB extends SQLite3
{
 function __construct($file)
  {
     $this->open($file);
  }
}

$db = new MyDB($dbnamex);
   
//$db = new SQLite3('/mrbcapi/database/SYSQITRDB.db');
$result = array();

$results = $db->query('SELECT Version FROM T_ProjectInfo');
while ($row = $results->fetchArray()) {
    //array_push($result,array('version'=>$row[0]));
    $result[] = $row;
}

echo json_encode(array("result"=>$result));
?>

