   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	   i eMicrosoft.TeamFoundation.Controls, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls%Microsoft.TeamFoundation.Controls.WPF 5   	    DBI Scripting4   .DBI_Scripting.Forms.Download.FrmCreateOESyntax   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   nWPF^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls     5   	   -   5          �Name$  frmCreateOESyntax��5   t    � Title$ Create OE Syntax��5   	   $	��430��6"   $	��768��6/    � WindowStartupLocation$ CenterScreen=�6;    � 
ResizeMode$ NoResize=�6`    � Icon$3 -/DBI Scripting;component/Images/reject_64.png��6v   .��5      � .+�5   
   �� $��Data file Path :��6   $
��Left=�6+   $��	11,21,0,0q�6F   $	��Top=�6Y    J� RenderTransformOrigin$ -3.74,-2.112&�6q   �� 5	   
   -   5         ��Name$ txtDataFileLocation��5	      $
��Left=�6/   $��23��6J   $��
103,24,0,0q�6V    �� TextWrapping$
 Wrap=�6j   $	��Top=�6~   $	��551��6�   �� 5
   
   -   5         ��Name$ btnBrowseExcelData��5
      $��Browse��6-   $
��Left=�6>   $��
659,24,0,0q�6Y   $	��Top=�6m   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   -   5        $ 
btnExecute��5      $��Execute��6%   $
��Left=�67   $��457,238,0,0q�6R   $	��Top=�6g   $	��108��6   $��31��6�   $ 0.822,-5.176&�6�   6�   �� 5   
   -   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��457,308,0,0q�6N   $	��Top=�6c   $	��102��6{   $ 8.079,-5.321&�6�   $��31��6�   6�   �� 5   
   -   5        	 ��Name$	 chkSelectAll��5      $��
Select All��6)   $
��Left=�6>   $��306,352,0,0q�6Y   $	��Top=�6n   6�   �� 5   
   $��	WrokSheet��6   $
��Left=�6$   $��
155,61,0,0q�6?   $	��Top=�6S   $ -3.74,-2.112&�6k   $   Xceed.Wpf.Toolkit.CheckListBox  5   
   -   5        
  Name$
 chkListBoxWorksheet��5      $
��Left=�6<   $	��286��6W   $��
105,88,0,0q�6d   $	��Top=�6x   $	��179��6�   6�   �� 5   
   $�� ��6   $
��Left=�6   $��362,138,0,0q�68   $	��Top=�6M   $��78��6e   $	��292��6q   .��5      � .+�5      �� -   5         ��Name$ 	lblStatus��5      $��	Status : ��6+   $
��Left=�6?   $��	10,25,0,0q�6Z   $	��Top=�6m   $��25��6�   $	��263��6�   �� 5      -	   5        $ lblProgress��5      $��Progress : ��6-   $
��Left=�6C   $��10,0,0,0q�6^   $	��Top=�6p   $��25��6�   $	��263��6�   5      5      �� 5   
   $��	File Name��6   $
��Left=�6$   $��
473,62,0,0q�6?   $	��Top=�6S   $ -3.74,-2.112&�6k   �� 5   
   -
   5        $ txtOESyntaxName��5      $
��Left=�6+   $��23��6F   $��
362,88,0,0q�6R   $
 Wrap=�6f   $	��Top=�6z   $	��292��6�   $��02.OE_SyntaxV1��6�   $��Center=�6�   $��Center=�6�   �� 5   
   -   5        $	 chkDontCreateVar��5      $��Don't Create Variables��6-   $
��Left=�6N   $��439,123,0,0q�6i   $	��Top=�6~   5      5       