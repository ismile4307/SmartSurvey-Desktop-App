<?php

//echo phpinfo();
include("include/db_connect.php");

class MyDB extends SQLite3
{
 function __construct()
  {
     $this->open('SYSMAPDB.db');
  }
}

//$db = new MyDB();
$db = new MyDB();
   if(!$db){
      echo $db->lastErrorMsg();
   } else {
      //echo "Opened database successfully\n";
   }

//**************************************************************************************
$r=array();
$r = $db->query('SELECT * FROM T_Question');

$i=array();
while($x= $r->fetchArray()){

	$i['project_id']=$x[0];
	break; 
}

DB_Util::deleteIfExist('questions', $i);
DB_Util::deleteIfExist('opt_attributes', $i);

//********************************Question Table******************************************************
      
//$db = new SQLite3('/mrbcapi/database/SYSQITRDB.db');
$result = array();

$results = $db->query('SELECT * FROM T_Question');
while ($row = $results->fetchArray()) {
    //array_push($result,array('version'=>$row[0]));
    $RespSaveData = array();
    
    $RespSaveData['project_id']=$row[0];
	$RespSaveData['qid']=$row[1];
	$RespSaveData['language_id']=$row[2];
	$RespSaveData['qdesc1']=$row[3];
	$RespSaveData['qdesc2']=$row[4];
	$RespSaveData['qdesc3']=$row[5];
	$RespSaveData['instruction_for_fi1']=$row[6];
	$RespSaveData['instruction_for_fi2']=$row[7];
	$RespSaveData['qtype']=$row[8];
	$RespSaveData['no_of_response_min']=$row[9];
	$RespSaveData['no_of_response_max']=$row[10];
	$RespSaveData['has_auto_response']=$row[11];
	$RespSaveData['has_logic_issue']=$row[12];
	$RespSaveData['has_filterred_attrib']=$row[13];
	$RespSaveData['has_logical_qes_label']=$row[14];
	$RespSaveData['has_message_logic']=$row[15];
	$RespSaveData['has_random_attribute']=$row[16];
	$RespSaveData['written_oe_inpaper']=$row[17];
	$RespSaveData['force_to_take_oe']=$row[18];
	$RespSaveData['has_logical_media_path']=$row[19];
	$RespSaveData['display_next_button']=$row[20];
	$RespSaveData['display_back_button']=$row[21];
	$RespSaveData['display_jump_button']=$row[22];
	$RespSaveData['display_exit_button']=$row[23];
	$RespSaveData['display_pause_button']=$row[24];
	$RespSaveData['order_tag']=$row[25];
	$RespSaveData['file_path']=$row[26];
	$RespSaveData['order_tag1']=$row[27];
	$RespSaveData['order_tag2']=$row[28];
	$RespSaveData['order_tag3']=$row[29];
	$RespSaveData['order_tag4']=$row[30];
	$RespSaveData['order_tag5']=$row[31];
	$RespSaveData['created_at']=date('Y-m-d H:i:s');
	
	$res = DB_Util::insert('questions', $RespSaveData);
        if (!$res) {
      		renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
    	}	
    
    
    //$result[] = $RespSaveData;
    
    if($RespSaveData['qtype']=='51')
    	break;
    
    //$result[] = $row;
}


//************************************Attribute Table**************************************************
      
//$db = new SQLite3('/mrbcapi/database/SYSQITRDB.db');
$result = array();

$results = $db->query('SELECT * FROM T_OptAttribute');
while ($row = $results->fetchArray()) {
	if($row[1]!="" && $row[2]!="")
	{
    //array_push($result,array('version'=>$row[0]));
    $RespSaveData = array();
    
    $RespSaveData['project_id']=$row[0];
	$RespSaveData['qid']=$row[1];
	$RespSaveData['language_id']=$row[2];
	$RespSaveData['attribute_label']=str_replace ("'","\'",$row[3]);
	$RespSaveData['attribute_value']=$row[4];
	$RespSaveData['attribute_order']=$row[5];
	$RespSaveData['take_openended']=$row[6];
	$RespSaveData['is_exclusive']=$row[7];
	$RespSaveData['link_id1']=$row[8];
	$RespSaveData['link_id2']=$row[9];
	$RespSaveData['min_value']=$row[10];
	$RespSaveData['max_value']=$row[11];
	$RespSaveData['force_and_msg_opt']=$row[12];
	$RespSaveData['created_at']=date('Y-m-d H:i:s');
		
	$res = DB_Util::insert('opt_attributes', $RespSaveData);
        if (!$res) {
      		renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
    	}	
    
    
    //$result[] = $RespSaveData;
    }
    //$result[] = $row;
}

//************************************Grid Attribute Table**************************************************
      
//$db = new SQLite3('/mrbcapi/database/SYSQITRDB.db');
$result = array();

$results = $db->query('SELECT * FROM T_GridInfo');
while ($row = $results->fetchArray()) {
	if($row[1]!="" && $row[2]!="")
	{
    //array_push($result,array('version'=>$row[0]));
    $RespSaveData = array();
    
    	$RespSaveData['project_id']=$row[0];
	$RespSaveData['qid']=$row[1];
	$RespSaveData['language_id']=$row[2];
	$RespSaveData['attribute_label']=str_replace ("'","\'",$row[3]);
	$RespSaveData['attribute_value']=$row[4];
	$RespSaveData['attribute_order']=$row[5];
	$RespSaveData['take_openended']=$row[6];
	$RespSaveData['is_exclusive']=$row[7];
	$RespSaveData['link_id1']=$row[8];
	$RespSaveData['link_id2']=$row[9];
	$RespSaveData['min_value']=$row[10];
	$RespSaveData['max_value']=$row[11];
	$RespSaveData['force_and_msg_opt']=$row[12];
	$RespSaveData['created_at']=date('Y-m-d H:i:s');
		
	$res = DB_Util::insert('grid_infos', $RespSaveData);
        if (!$res) {
      		renderJSON(array('success' => false, 'message' =>'Could not save responses'. DB_Util::$currentQuery));
    	}	
    
    
    //$result[] = $RespSaveData;
    }
    //$result[] = $row;
}

//echo json_encode(array("result"=>$result));

echo "Table Preperation completed";

function renderJSON($data = array())
{
    header('Content-Type: application/json');
    echo json_encode($data);
    exit();
}

?>