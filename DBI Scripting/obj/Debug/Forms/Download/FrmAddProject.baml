   M S B A M L   `   `   `  ����  DBI Scripting0    *DBI_Scripting.Forms.Download.FrmAddProject   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  FrmAddProject1��5   	    � Title$ Add Project��5   	   $��492.832��6   $��782.779��6.    � WindowStartupLocation$ CenterScreen=�6]   .��5      � .+�5   
   �� $�� ��6   $
��Left=�6   $��	10,10,0,0q�68   $	��Top=�6K   $	��169��6c   $	��756��6p   .��5      � .+�5      �� $��Project Name :��6   $
��Left=�61   $��	29,10,0,0q�6L   $	��Top=�6_   �� 5	      $��Project Code :��6   $
��Left=�61   $��
381,10,0,0q�6L   $	��Top=�6`   �� 5
      -   5         ��Name$ txtProjectName��5
      $
��Left=�62   $��23��6M   $��
120,14,0,0q�6Y    �� TextWrapping$
 Wrap=�6m   $�� ��6�   $	��Top=�6�   $	��226��6�   �� 5      -   5        $ txtProejctCode��5      $
��Left=�62   $��23��6M   $��
470,13,0,0q�6Y   $
 Wrap=�6m   $�� ��6�   $	��Top=�6�   $	��213��6�   �� 5      $��Script Name :��6   $
��Left=�60   $��	35,59,0,0q�6K   $	��Top=�6^   �� 5      $��Start Date : ��6   $
��Left=�60   $��
381,59,0,0q�6K   $	��Top=�6_    J� RenderTransformOrigin$ 0.518,2.496&�6w   �� 5      -   5        $ txtScriptFileName��5      $
��Left=�65   $��23��6P   $��
120,63,0,0q�6\   $
 Wrap=�6p   $�� ��6�   $	��Top=�6�   $	��226��6�   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ dtpStartDate��5      $
��Left=�63   $��
470,61,0,0q�6N   $	��Top=�6b   $	��213��6z   �� 5      -   5         ��Name$ btnClose��5      $��Close��6+   $
��Left=�6;   $��608,115,0,0q�6V   $	��Top=�6k   $��75��6�   6�   �� 5      -   5        $ btnSave��5      $
��Save��6*   $
��Left=�69   $��507,115,0,0q�6T   $	��Top=�6i   $��75��6�   6�   �� 5      -   5        $ 
btnRefresh��5      $��Refresh��6-   $
��Left=�6?   $��406,115,0,0q�6Z   $	��Top=�6o   $��75��6�   �� 5      $��	Status : ��6   $
��Left=�6,   $��
65,105,0,0q�6G   $	��Top=�6[   �� 5      -	   5         ��Name$ comProjectStatus��5      $
��Left=�65   $��120,109,0,0q�6P   $	��Top=�6e   $	��120��6}   5      5      �� 5   
   $�� ��6   $
��Left=�6   $��
10,184,0,0q�68   $	��Top=�6L   $	��269��6d   $	��756��6q   .��5      &   System.Windows.Controls.DataGrid  -
   5        	  Name$	 dataGridProject��5      $
��Left=�60   $	��Top=�6K   $	��245��6c   $	��746��6p   $��0,0,-2,0q�6|   
   SelectionMode$
 Single=�6�   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�      CanUserSortColumns .     CanUserReorderColumns .     CanUserResizeRows .     CanUserAddRows .     
IsReadOnly . 64  5      5      5      