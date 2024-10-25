using DBI_Scripting.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class GetData
    {
        public bool hasFilterAttribute(String projectId, String qId, ConnectionDB connQntrDB)
        {
            try
            {
                DBHelper myDBHelper = new DBHelper();

                bool bResult = false;

                String query = "SELECT * FROM T_OptAttrbFilter WHERE QId='" + qId + "' AND ProjectId=" + projectId;

                DataTable dt = myDBHelper.getQntrTableData(query, connQntrDB);
                if (dt.Rows.Count > 0)
                {
                    bResult = true;
                }

                return bResult;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //Get the attribute list having rotation/randomization
        //public List<MyAttribute> getAttribute(String projectId, String respondentId, String qId, String hasRandomization, ConnectionDB connQntrDB, ConnectionDB connAnsDB)
        //{
        //    // TODO Auto-generated method stub
        //    try
        //    {



        //        List<MyAttribute> lstAttribute = new List<MyAttribute>();
        //        List<MyAttribute> lstAttributeTemp = new List<MyAttribute>();

        //        List<List<MyAttribute>> listOfListAttributeTemp;
        //        List<List<MyAttribute>> listOfListAttribute;

        //        bool hasGroup = false;

        //        String query = "SELECT * FROM T_OptAttribute " + "WHERE ProjectId="
        //                + projectId + " AND " + "QId='" + qId + "'"; // Order By AttributeOrder";

        //        Cursor crs = quesDbAdapter.getData(query);
        //        while (crs.moveToNext())
        //        {
        //            String tempAttributeLabel = crs.getString(crs.getColumnIndex(Config.ATTRIBUTE_TEXT_FIELD));//get the attribute text field name
        //            String myAttributeLabel = "";

        //            if (!crs.getString(crs.getColumnIndex("GroupName")).equals("") && crs.getString(crs.getColumnIndex("GroupName")) != null)
        //                hasGroup = true;

        //                myAttributeLabel = tempAttributeLabel;

        //            //Get the Grid info if exist
        //            List<MyGridAttribute> gridAttributes = new List<MyGridAttribute>();
        //            String linkId = crs.getString(crs.getColumnIndex("LinkId2"));

        //            String gridAttribFilterQid = crs.getString(crs.getColumnIndex("FilterQid"));
        //            String gridAttribFilterType = crs.getString(crs.getColumnIndex("FilterType"));
        //            String gridAttribExcepValue = crs.getString(crs.getColumnIndex("ExcepValue"));


        //            if (linkId!="" && linkId != null)
        //            {
        //                gridAttributes = getGridAttribute(projectId, respondentId, qId, linkId, gridAttribFilterQid, gridAttribFilterType, gridAttribExcepValue, hasRandomization, quesDbAdapter, ansDbAdapter);
        //            }

        //            //No need to add the attribute if label is blank
        //            //if(!myAttributeLabel.equals("")) {
        //            // Prepare the attribute list
        //            lstAttributeTemp.Add(new Attribute(myAttributeLabel,
        //                    crs.getString(crs.getColumnIndex("AttributeValue")),
        //                    Integer.parseInt(crs.getString(crs.getColumnIndex("AttributeOrder"))),
        //                    crs.getString(crs.getColumnIndex("TakeOpenended")),
        //                    crs.getString(crs.getColumnIndex("IsExclusive")),
        //                    crs.getString(crs.getColumnIndex("LinkId1")),
        //                    crs.getString(crs.getColumnIndex("LinkId2")),
        //                    crs.getString(crs.getColumnIndex("MinValue")),
        //                    crs.getString(crs.getColumnIndex("MaxValue")),
        //                    crs.getString(crs.getColumnIndex("ForceAndMsgOpt")),
        //                    gridAttributes));

        //            //}
        //        }


        //        if (hasGroup == true)
        //        {
        //            listOfListAttributeTemp = new List<List<MyAttribute>>();
        //            listOfListAttribute = new List<List<MyAttribute>>();
        //            List<Attribute> listOfAttribute = new List<MyAttribute>();

        //            bool firstTime = true;
        //            String previousGroupId = "";

        //            for (int x = 0; x < lstAttributeTemp.size(); x++)
        //            {
        //                String currentGroupId = lstAttributeTemp.get(x).minValue;

        //                if (currentGroupId.Equals(previousGroupId))
        //                {
        //                    listOfAttribute.Add((lstAttributeTemp.get(x)));
        //                }
        //                else
        //                {
        //                    if (firstTime == false)
        //                        listOfListAttributeTemp.Add(listOfAttribute);

        //                    listOfAttribute = new List<Attribute>();
        //                    listOfAttribute.Add((lstAttributeTemp.get(x)));

        //                    previousGroupId = lstAttributeTemp.get(x).minValue;

        //                    firstTime = false;
        //                }
        //            }
        //            // Add the last attribute list
        //            listOfListAttributeTemp.Add(listOfAttribute);

        //            // 1st digit Group
        //            // 2nd digit Attribute
        //            if (hasRandomization.Equals("10") && listOfListAttributeTemp.size() > 1)
        //            {
        //                //Rotate the Group
        //                listOfListAttribute = rotateGroup(respondentId, listOfListAttributeTemp);


        //            }
        //            else if (hasRandomization.Equals("20") && listOfListAttributeTemp.size() > 1)
        //            {
        //                //Randomize the group
        //                listOfListAttribute = randomizeGroup(respondentId, listOfListAttributeTemp);


        //            }
        //            else if (hasRandomization.Equals("01") && listOfListAttributeTemp.size() > 1)
        //            {
        //                //Rotate the attribute but not group
        //                listOfListAttribute = new List<List<Attribute>>();
        //                for (int x = 0; x < listOfListAttributeTemp.size(); x++)
        //                {
        //                    listOfListAttribute.add(rotateAttribute(respondentId, listOfListAttributeTemp.get(x)));
        //                }

        //            }
        //            else if (hasRandomization.equals("02") && listOfListAttributeTemp.size() > 1)
        //            {
        //                //Randomize the attribute but not group
        //                listOfListAttribute = new List<List<Attribute>>();
        //                for (int x = 0; x < listOfListAttributeTemp.size(); x++)
        //                {
        //                    listOfListAttribute.add(randomizeAttribute(respondentId, listOfListAttributeTemp.get(x)));
        //                }

        //            }
        //            else if (hasRandomization.equals("11") && listOfListAttributeTemp.size() > 1)
        //            {
        //                //Rotate group and the attribute

        //                List<List<Attribute>> listOfListAttributeTemp1 = rotateGroup(respondentId, listOfListAttributeTemp);
        //                listOfListAttribute = new List<List<Attribute>>();
        //                for (int x = 0; x < listOfListAttributeTemp1.size(); x++)
        //                {
        //                    listOfListAttribute.add(randomizeAttribute(respondentId, listOfListAttributeTemp1.get(x)));
        //                }

        //            }
        //            else if (hasRandomization.equals("22") && listOfListAttributeTemp.size() > 1)
        //            {
        //                //Randomize group and the attribute
        //                List<List<Attribute>> listOfListAttributeTemp1 = randomizeGroup(respondentId, listOfListAttributeTemp);
        //                listOfListAttribute = new List<List<Attribute>>();
        //                for (int x = 0; x < listOfListAttributeTemp1.size(); x++)
        //                {
        //                    listOfListAttribute.add(randomizeAttribute(respondentId, listOfListAttributeTemp1.get(x)));
        //                }

        //            }
        //            else
        //            {
        //                listOfListAttribute = listOfListAttributeTemp;
        //            }

        //            for (int x = 0; x < listOfListAttribute.size(); x++)
        //            {
        //                List<Attribute> myAttribute = listOfListAttribute.get(x);
        //                for (int y = 0; y < myAttribute.size(); y++)
        //                {
        //                    lstAttribute.add((myAttribute.get(y)));
        //                }
        //            }

        //        }
        //        else
        //        {
        //            if (hasRandomization.equals("1") && lstAttributeTemp.size() > 1)
        //            {
        //                lstAttribute = rotateAttribute(respondentId, lstAttributeTemp);
        //            }
        //            else if (hasRandomization.equals("2") && lstAttributeTemp.size() > 1)
        //            {
        //                lstAttribute = randomizeAttribute(respondentId, lstAttributeTemp);
        //            }
        //            else
        //                lstAttribute = lstAttributeTemp;
        //        }
        //        crs.close();
        //        quesDbAdapter.close();
        //        // db.close();
        //        return lstAttribute;
        //    }
        //    catch (SQLException e)
        //    {
        //        e.printStackTrace();
        //        return null;
        //    }
        //}
    }
}
