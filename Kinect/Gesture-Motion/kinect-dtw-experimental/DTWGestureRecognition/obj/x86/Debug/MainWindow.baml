   M S B A M L   `   `   `  ����  DTWGestureRecognition&     DTWGestureRecognition.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$!  Gesture Tracking with Audio��5   	   $	��768��6-   $
��1024��6:   .��5      � -   5        $��mainGrid��5   %   $	��737��6   $
��1010��6   .+�5
   
   �� -   5        $��skeletonCanvas��5
   *   $	��823��6   $	��617��6   
���   �}�. $��172,0,0,120q�6g   $
��Left=�6|   �� 5   
   -   5        $��stackPanel2��5   =   $	��104��6   $
��Left=�6"   $	��Top=�6P   $	��510��6h   $��485,633,0,0q�6t   .+�5      �� -   5        $��playTextBox��5   $   $��14(�6   $(��"Select audio file then click play.��67   |�.  $��0q�6s    �� TextAlignment$ Center=�6�   �� 5      -   5        $��selectAudioBtn��5   =   $��Select Audio��6   {�. 6S   �� 5      -   5        $��fileNameTextBox��5   $   $��14(�6   $��
File name.��6;   |�.  $��0q�6_   $ Center=�6s   �� 5      -   5        $��playAudioBtn��5   6   $
��Play��6   {�. 6J   �� 5      -	   5        $��pauseAudioBtn��5   7   $��Pause��6   {�. 6L   5      �� 5   
   -
   5         ��Name$ results��5       $��40(�6   |�.  $��0q�6C   $ Center=�6W   $��10,633,530,10q�6n   $��Center=�6�   $��Center=�6�   $��Gesture Name��6�   �� 5   
   -   5        $��stackPanel1��5   =   $	��182��6   $
��Left=�6"   $	��Top=�6P   $	��157��6h   $��
10,230,0,0q�6t   .+�5      �� -   5         ��Name$ gestureList��5      .@�5      ��  �� 
IsSelected . $��@Right hand swipe left��61   �� 5      $��@Right hand swipe right��6   �� 5      $��@Left hand swipe left��6   �� 5      $��@Left hand swipe right��6   �� 5      $��@Two hands zoom in��6   �� 5      $��@Two hands zoom out��6   �� 5      $��@Right hand wave��6   �� 5       $��@Left hand wave��6   �� 5!      $��@Right hand pull down��6   �� 5"      $��@Left hand pull down��6   �� 5#      $��@Right hand push up��6   �� 5$      $��@Left hand push up��6   �� 5%      $��@Both hands pull down��6   �� 5&      $��@Both hands push up��6   �� 5'      $ ��@Get on down at the disco!��6   5(      �� 5)      -   5         ��Name$ 
dtwCapture��5)      $��Capture��6)   6;   �� 5*      -   5        $ currentBufferFrame��5*   $   $��14(�6   |�.  $��0q�6R   $��Frame��6f   $ Center=�6s   �� 5+      -   5        $ 	frameRate��5+   $   $��14(�6   |�.  $��0q�6I   $�� ��6]   $ Center=�6e   �� 5,      -   5        $ status��5,   $   $��14(�6   |�.  $��0q�6F   $��Status��6Z   $ Center=�6h   �� 5-      -   5        $ dtwStore��5-      $��Store��6'   {�.  6I   �� 5.      -   5        $ dtwSaveToFile��5.      $��Save to file��6,   {�. 6T   �� 5/      -   5        $ dtwLoadFile��5/      $��Load gesture file��6*   {�. 6W   �� 50      -   5        $ dtwRead��50      $
��Read��6&   {�.  6J   52      53      54      