   M S B A M L   `   `   `  ����  DTWGestureRecognition&     DTWGestureRecognition.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$!  Gesture Tracking with Audio��5   	   $	��768��6-   $
��1024��6:   .��5      � -   5        $��mainGrid��5   %   $	��737��6   $
��1010��6   .+�5   
   �� -   5        $��
videoImage��5   )   $	��500��6   $	��375��6   $��0,362,510,0q�6;   $��Right=�6P   �� 5   
   -   5        $��skeletonCanvas��5   *   $	��500��6   $	��375��6   
���   �}�. $��508,2,0,360q�6g   $
��Left=�6|   6�   �� 5   
   -   5        $��
depthImage��5   )   $	��500��6   $	��375��6   $��	0,362,2,0q�6;   $��Right=�6N   �� 5
   
   -   5        $��stackPanel1��5
   =   $	��356��6   $
��Left=�6"   $	��Top=�6P   $	��500��6h   .+�5      �� -   5        $��gestureList��5      .@�5      ��  �� 
IsSelected . .��61   @Right hand swipe left6I   �� 5      .��6   @Right hand swipe right68   �� 5      .��6   @Left hand swipe left66   �� 5      .��6   @Left hand swipe right67   �� 5      .��6   @Two hands zoom in63   �� 5      .��6   @Two hands zoom out64   �� 5      .��6   @Right hand wave61   �� 5      .��6   @Left hand wave60   �� 5      .��6   @Right hand pull down66   �� 5      .��6   @Left hand pull down65   �� 5      .��6   @Right hand push up64   �� 5      .��6   @Left hand push up63   �� 5      .��6   @Both hands pull down66   �� 5      .��6   @Both hands push up64   �� 5      .��6   @Get on down at the disco!6;   5      �� 5      -   5        $��
dtwCapture��5      $��Capture��6'   69   �� 5      -	   5        $��currentBufferFrame��5   $   $��14(�6   |�.  $��0q�6P   $��Frame��6d    �� TextAlignment$ Center=�6q   �� 5      -
   5        $��	frameRate��5   $   $��14(�6   |�.  $��0q�6G   $��Results��6[   $ Center=�6j   �� 5      -   5        $��results��5   $   $��14(�6   |�.  $��0q�6E   $�� ��6Y   $ Center=�6a   �� 5       -   5        $��status��5    $   $��14(�6   |�.  $��0q�6D   $��Status��6X   $ Center=�6f   �� 5!      -   5        $��dtwStore��5!      $��Store��6%   {�.  6G   �� 5"      -   5        $��dtwSaveToFile��5"      $��Save to file��6*   {�. 6R   �� 5#      -   5        $��dtwLoadFile��5#      $��Load gesture file��6(   {�. 6U   �� 5$      -   5        $��dtwRead��5$      $
��Read��6$   {�.  6H   �� 5%      -   5        $��playTextBox��5%   $   $��14(�6   $(��"Select audio file then click play.��67   |�.  $��0q�6s   $ Center=�6�   �� 5&      -   5        $��selectAudioBtn��5&   =   $��Select Audio��6   {�. 6S   �� 5'      -   5        $��fileNameTextBox��5'   $   $��14(�6   $��
File name.��6;   |�.  $��0q�6_   $ Center=�6s   �� 5(      -   5        $��playAudioBtn��5(   6   $
��Play��6   {�. 6J   �� 5)      -   5        $��pauseAudioBtn��5)   7   $��Pause��6   {�. 6L   5*      5+      5,      