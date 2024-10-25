<?php
include("include/db_connect.php");



class MyDB extends SQLite3
{
 function __construct()
  {
     $this->open('SYSTWSDB.db');
  }
}

$db = new MyDB();
   if(!$db){
      echo $db->lastErrorMsg();
   } else {
      //echo "Opened database successfully\n";
   }

//**************************************************************************************

//*****************Create ColumnHeader ********************

$columnName=array();

array_push($columnName,"RespondentId","Latitude","Longitude","SurveyDateTime");

//print_r($columnName);


$qryResult0=$db->query("SELECT T_QType.ID, T_QType.ResponseType FROM T_QType WHERE T_QType.ResponseType!='0'");

$QTypeIdVsResponseType=array();

while($row= $qryResult0->fetchArray()){

	$QTypeIdVsResponseType[$row[0]]=$row[1];
}

//print_r($QTypeIdVsResponseType);

//Delete all data from report_qtype

$connDelete=DB_Util::connect();

// Check connection
if ($connDelete->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// sql to delete a record
$sql = "DELETE FROM export_qtype";

if ($connDelete->query($sql) === TRUE) {
  //echo "Record deleted successfully";
} else {
  echo "Error deleting record: " . $connDelete->error;
}

$connDelete->close();

//***********************************************************

$connInsert=DB_Util::connect();

$qryResult1 = $db->query("SELECT T_Question.ProjectId, T_Question.QId, T_Question.QDesc3, T_Question.QType FROM T_Question INNER JOIN T_QType ON T_Question.QType = T_QType.ForQuesLink WHERE (T_QType.ShowInReport='1' OR T_Question.QType=51 ) AND T_Question.LanguageId='2' Order by T_Question.OrderTag");

while($row= $qryResult1->fetchArray()){

    $projectId=$row[0];
	$qid=$row[1];
	$qDesc=$row[2];
	
	if($row['QType']==51)
	    break;
	    
	$sql="";    
	
	if($QTypeIdVsResponseType[$row['QType']]==2)
	{
	    $count=getAttributeNumber($projectId,$qid,$qDesc,$db);
	    
	    while($r= $count->fetchArray())
	        {
	            array_push($columnName,$qid.'_'.$r['AttributeOrder']);
	        }
	        
	    $sql = "INSERT INTO export_qtype (qid, qtype, rtype) VALUES ('".$row['QId']."', ".$row['QType'].", 2)";
	}
	else
	{
	    array_push($columnName,$row['QId']);
	     $sql = "INSERT INTO export_qtype (qid, qtype, rtype) VALUES ('".$row['QId']."', ".$row['QType'].", 1)";
	}
	
	//**************************
	
    if ($connInsert->query($sql) === TRUE) {
      //echo "New record created successfully";
    } else {
      echo "Error: " . $sql . "<br>" . $conn->error;
    }
    //***************************************
	
}

$connInsert->close();


array_push($columnName,"Intv_Type","FICode","FSCode","AccompaniedBy","BackCheckedBy","ScriptVersion","SyncDateTime","Status","TabId");

//print_r(json_encode($columnName));


//-----------------------------------------------------------------------------
$columnHeader = '';  

for ($x = 0; $x < count($columnName); $x++) {
  $columnHeader.='"' . $columnName[$x] . '"' . ",";
}

//-----------------------------------------------------------------------------
if (isset($_POST['from_date'])) {
$from_date = $_POST['from_date'];
}
if (isset($_POST['to_date'])) {
$to_date = $_POST['to_date'];
}

//print_r($from_date);
//-----------------------------------------------------------------------------

$conn=DB_Util::connect();

$query="SELECT * FROM `interview_infos` WHERE `project_id`=10320 AND `intv_type`='1' AND `survey_start_at` BETWEEN '".$from_date." 00:00:00' AND '".$to_date." 23:59:59' AND `deleted_at` IS NULL";



$sql=mysqli_query($conn,$query,MYSQLI_USE_RESULT);
$dataForExcel=$columnHeader;

$filename = "CapiData.txt";
$file = fopen($filename,"w");


while($rows1 = mysqli_fetch_assoc($sql)) {
    
    $id=$rows1['id'];
    
    //echo $id;
    
    $query2="SELECT answers.q_id, answers.response, answers.q_order, answers.resp_order, export_qtype.qtype, export_qtype.rtype FROM answers INNER JOIN export_qtype ON answers.q_id=export_qtype.qid WHERE answers.project_id=10320 AND answers.interview_info_id=".$id.";";
    
    //echo $query2;
    $conn2=DB_Util::connect();
    $myVarName='';
    $myData=array();
    
    $myData['RespondentId']=$rows1['respondent_id'];
    $myData['Latitude']=$rows1['latitude'];
    $myData['Longitude']=$rows1['longitude'];
    $myData['SurveyDateTime']=$rows1['survey_start_at'];
    $myData['Intv_Type']=$rows1['intv_type'];
    $myData['FICode']=$rows1['fi_code'];
    $myData['FSCode']=$rows1['fs_code'];
    $myData['AccompaniedBy']=$rows1['accompanied_by'];
    $myData['BackCheckedBy']=$rows1['back_checked_by'];
    $myData['ScriptVersion']=$rows1['script_version'];
    $myData['SyncDateTime']=$rows1['created_at'];
    $myData['Status']=$rows1['status'];
    $myData['TabId']=$rows1['tab_id'];
    
    
    $sql2=mysqli_query($conn2,$query2,MYSQLI_USE_RESULT);
    
    while($rows2 = mysqli_fetch_assoc($sql2)) {
        
        if ($rows2['rtype']==2)
            $myData[$rows2['q_id']."_".$rows2['resp_order']]=$rows2['response'];
        else
            $myData[$rows2['q_id']]=$rows2['response'];
            
        
        
    }

    //print_r(json_encode($myData));

    mysqli_close($conn2);
    
    $singleData='';
    
    //echo count($columnName);

    for($i=0;$i<count($columnName);$i++)
    {
        if(array_key_exists($columnName[$i],$myData))
            $singleData.='"'.str_replace(
                array("\n", "\r"), '', $myData[$columnName[$i]]
                ).'"'.",";
        else
            $singleData.=",";
    }

    //echo $singleData;
    

    fwrite($file,$singleData.PHP_EOL);
    
    //$dataForExcel.="\n".$singleData;
    //break;
}

fclose($file);

//header("Content-type: application/octet-stream");  
//header("Content-Disposition: attachment; filename=User_Detail.csv");  
//header("Pragma: no-cache");  
//header("Expires: 0");  

//echo ucwords($dataForExcel);


header('Content-Description: File Transfer');
header('Content-Disposition: attachment; filename='.basename($filename));
header('Expires: 0');
header('Cache-Control: must-revalidate');
header('Pragma: public');
header('Content-Length: ' . filesize($filename));
header("Content-Type: text/plain");
readfile($filename);


mysqli_close($conn);    
    
// functoin for getting attribute list for a question.
function getAttributeNumber($ProjectId, $QId, $QDesc3,$db)
{
    if ($QDesc3 != "")
        $QId = $QDesc3;

    return $db->query("SELECT AttributeValue, AttributeOrder FROM T_OptAttribute where ProjectId=" . $ProjectId . " AND QId ='" . $QId . "' AND LanguageId='2'");
}



//echo $i['project_id'];

//$conn=DB_Util::connect();        

//$query="SELECT * FROM `interview_infos` WHERE `project_id`=23817 AND `intv_type`='1' AND `survey_start_at` BETWEEN '2017-10-21 00:00:00' AND '2017-10-21 23:59:59' AND `deleted_at` IS NULL";



//$row=mysqli_query($query);

//$posts=Array();
