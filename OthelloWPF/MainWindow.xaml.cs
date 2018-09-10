using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;
using Othello;

namespace OthelloWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PROPERTIES, ATTRIBUTES
        //private OthelloGame oGame;
        private OthelloAdapter.Othello OthelloGame;
        private OthelloToken[,] oBoard;

        private Othello.OthelloGamePlayer oCurrentPlayer;
        private int oTurn;
        private List<OthelloToken> oFlipList;
        private bool oIsInvalidMove;
        private bool IsHumanWhiteChecked = false;

        private GameDifficultyMode DifficultyMode { get; set; }

        private const string oInvalidMove = "Invalid Move.";
        private const string sEasy = "Easy";
        private const string sMedium = "Medium";
        private const string sHard = "Hard";
        private const double GridSize = 40.0;
        private const string playerAName = "PlayerA";
        private const string playerBName = "PlayerB";
        private const string playerHumanName = "Human";
        private const string playerAIName = "Computer";

        private Queue<bool> IsAnimating;
        private double OthelloTokenSize { get; set; }

        #endregion

        #region CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();

            //link up and initialize Othello Module for game logic and data management
            InitializeOthelloGame();
 
            //initialize UX for Othello
            InitOtelloBoard();

            this.Height = GridSize * 15;
            this.Width = GridSize * 15;
        }
        #endregion

        #region INITIALIZERS

        /// <summary>
        /// Initialize Game Elements of Othello
        /// </summary>
        private void InitializeOthelloGame()
        {            
            try
            {
                //create a new game by default
                //oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, false);
                OthelloGame = new OthelloAdapter.OthelloAdapter();
                OthelloGame.GameCreateNewHumanVSHuman(playerAName, playerBName, OthelloPlayerKind.White, false);

            }
            catch(Exception e)
            {
                MessageBox.AppendText(string.Format("{0}\n",e.Message));
            }
            finally
            {
                //set rendering othello token size
                OthelloTokenSize = GridSize * 0.8;
                oFlipList = new List<OthelloToken>();
                oIsInvalidMove = false;

                MessageBox.AppendText(string.Format("{0}\n", "Initialized:Success. Human vs. Human"));
            }
        }

        /// <summary>
        /// initialize the othello board grid lines and cells
        //  TODO: intialize that can scale the UX larger
        /// </summary>
        private void InitOtelloBoard()
        {
            OthelloGrid.Children.Clear();

            OthelloGrid.Width = GridSize*8;
            OthelloGrid.Height = GridSize*8;

            for (int i = 0; i < OthelloBoard.BoardSize; i++)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                coldef.Width = new GridLength(GridSize, GridUnitType.Star);
                OthelloGrid.ColumnDefinitions.Add(coldef);
            }

            for (int i = 0; i < OthelloBoard.BoardSize; i++)
            {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = new GridLength(GridSize, GridUnitType.Star);
                OthelloGrid.RowDefinitions.Add(rowdef);
            }

            //initialize buttons in each grid row and column for interactivity
            for (int i = 0; i < OthelloGrid.ColumnDefinitions.Count; i++)
                for (int j = 0; j < OthelloGrid.RowDefinitions.Count; j++)
                {
                    Button bt = CreateCellButton(i, j);
                    OthelloGrid.Children.Add(bt);
                }

            //intialize only when fully loaded
            OthelloGrid.Loaded += OthelloGrid_Loaded;
            IsAnimating = new Queue<bool>();
        }

        private Button CreateCellButton(int i, int j)
        {
            Button bt = new Button();
            bt.Background = new SolidColorBrush((Color)FindResource("OthelloBoardColor"));
            bt.BorderBrush = new SolidColorBrush((Color)FindResource("OthelloBoardBorderColor"));
            bt.Style = (Style)FindResource("OthelloBoardCellStyle");


            bt.IsEnabled = true;
            bt.Width = OthelloGrid.ColumnDefinitions[0].Width.Value;
            bt.Height = OthelloGrid.RowDefinitions[0].Height.Value;
            bt.BorderThickness = new Thickness(1);
            bt.Padding = new Thickness(2);
            bt.Click += ClickOthelloCell;
            bt.ToolTip = string.Format("({0},{1})", i, j);
            bt.SetValue(Grid.ColumnProperty, i);
            bt.SetValue(Grid.RowProperty, j);
            return bt;
        }

        /// <summary>
        ///  Initialize the first laytout of black and white tops    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        void OthelloGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Initialize the combo box when loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Difficulty_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add(sEasy);
            data.Add(sMedium);
            data.Add(sHard);

            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;

        }


        #endregion

        #region UI HANDLERS
        /// <summary>
        /// create new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickNew(object sender, RoutedEventArgs e)
        {
            InitializeOthelloGame();
            Refresh();
        }

        /// <summary>
        /// create a new game specific for A.I
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickNewAIGame(object sender, RoutedEventArgs e)
        {
            MessageBox.Clear();
            try
            {
                if (IsHumanWhiteChecked)
                {
                    OthelloGame.GameCreateNewHumanVSAI(playerHumanName, playerAIName, OthelloPlayerKind.White,IsHumanWhiteChecked, false, DifficultyMode);
                }
                else
                {
                    OthelloGame.GameCreateNewHumanVSAI(playerAIName, playerHumanName, OthelloPlayerKind.Black,IsHumanWhiteChecked, false, DifficultyMode);
                }
            }
            catch (Exception exception)
            {
                //TODO: possible to redirect messages from Trace to here?
                MessageBox.AppendText(string.Format("{0}\n", exception.Message));
            }
            finally
            {
                MessageBox.AppendText(string.Format("{0}\n", "AI Game Initialized:Success"));
                Refresh();
            }
        }

        /// <summary>
        /// load a previously saved game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickLoad(object sender, RoutedEventArgs e)
        {
            //oGame.GameLoad();
            OthelloGame.GameLoad();

            //TODO maybe design issue: OthelloGame does not support the loading of difficulty modes
            //switch(oGame.GameDifficultyMode)
            switch(OthelloGame.GameGetDifficultyMode())
            {
                case GameDifficultyMode.Easy:
                    Difficulty_ComboBox.SelectedIndex = 0;
                    break;
                case GameDifficultyMode.Medium:
                    Difficulty_ComboBox.SelectedIndex = 1;
                    break;
                case GameDifficultyMode.Hard:
                    Difficulty_ComboBox.SelectedIndex = 2;
                    break;
                default:
                    Difficulty_ComboBox.SelectedIndex = 0;
                    break;
            }

            Refresh();
        }

        /// <summary>
        /// save a current game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickSave(object sender, RoutedEventArgs e)
        {
            //oGame.GameSave();
            OthelloGame.GameSave();
            Refresh();
        }

        /// <summary>
        /// Undo a move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickUndo(object sender, RoutedEventArgs e)
        {
            //oGame.GameUndo();
            OthelloGame.GameUndo();
            Refresh();
        }

        /// <summary>
        /// Redo a move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickRedo(object sender, RoutedEventArgs e)
        {
            //oGame.GameRedo();
            OthelloGame.GameRedo();
            Refresh();
        }

        /// <summary>
        /// Handler for when each othello cell is clicked.
        /// TODO add sound effect when cell is clicked/flipped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickOthelloCell(object sender, RoutedEventArgs e)
        {
            int GameX = Convert.ToInt32( ((Button)e.Source).GetValue(Grid.ColumnProperty));
            int GameY = Convert.ToInt32( ((Button)e.Source).GetValue(Grid.RowProperty));

            Trace.WriteLine(string.Format("Clicked {0},{1}",
               GameX,
               GameY));
            
            //make a move
            //oFlipList = oGame.GameMakeMove(GameX, GameY, oCurrentPlayer);
            oFlipList = OthelloGame.GameMakeMove(GameX, GameY, oCurrentPlayer, out oIsInvalidMove);

            //oGame.DebugGameBoard();
            //oIsInvalidMove = oFlipList.Count() < 1 ? true : false;

            Refresh();           
        }

        /// <summary>
        /// To change human player to white for an A.I game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhiteHumanClick(object sender, RoutedEventArgs e)
        {

            if (((CheckBox)sender).IsChecked.Value)
                IsHumanWhiteChecked = true;
            else
                IsHumanWhiteChecked = false;

            /// TODO: could be a bug when an alternate game is saved but loaded in another alternate game
        }

        /// <summary>
        /// When difficulty menu is changed
        /// </summary>
        /// <remarks>
        /// Can change the difficulty in the middle of a game or after loading or saving.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Difficulty_Changed(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            string value = comboBox.SelectedItem as string;

            switch (value)
            {
                case sEasy:
                    DifficultyMode = GameDifficultyMode.Easy;
                    break;
                case sMedium:
                    DifficultyMode = GameDifficultyMode.Medium;
                    break;
                case sHard:
                    DifficultyMode = GameDifficultyMode.Hard;
                    break;
                default:
                    DifficultyMode = GameDifficultyMode.Easy;
                    break;
            }

            if (OthelloGame != null)
                OthelloGame.GameSetDifficultyMode(DifficultyMode);
                //oGame.GameDifficultyMode = DifficultyMode;

        }

        #endregion

        #region REFRESH, UPDATES
        //  a data-UI refresh
        private void Refresh()
        {
            UpdateGame();
            RenderGameInfo();

            oFlipList.Clear();
            oIsInvalidMove = false;
        }

        // updates made by or to game engine
        private void UpdateGame()
        {
            //oCurrentPlayer = oGame.GameUpdatePlayer();
            oCurrentPlayer = OthelloGame.GameUpdatePlayer();

            //oTurn = oGame.GameUpdateTurn();
            oTurn = OthelloGame.GameUpdateTurn();

            //oBoard = (OthelloToken[,])oGame.GameGetBoardData(OthelloBoardType.TokenMatrix);
            oBoard = OthelloGame.GameGetBoardData();
        }

        #endregion

        #region RENDERERS

        // Rendering with animation (move with delay) for better user feedback
        private void RenderGameInfo()
        {
            CanvasDrawingArea.Children.Clear();         
            RenderGameMetaData();
            RenderBoard();
        }

        //render the OthelloUX Board
        private void RenderBoard()
        {
            foreach(OthelloToken t in oBoard)
            {
                RenderCell(oBoard[t.X, t.Y], t.X, t.Y);
                SetOthelloUXCellAccess(t.X, t.Y);
            }
        }

        // Render a cell with a othello token given grid coordinates      
        private void RenderCell(OthelloToken ob, int x, int y)
        {
            bool IsAnimate = false;

            //get an item from the grid
            var item = LogicalTreeHelper
                            .GetChildren(OthelloGrid)
                            .OfType<Button>()
                            .Select(brd => brd)
                            .Where(a => Grid.GetColumn(a) == x && Grid.GetRow(a) == y).Single();

            //Get a position inside a cell
            Point topLeft = item.PointToScreen(
                new Point((OthelloGrid.ColumnDefinitions[0].Width.Value - OthelloTokenSize) / 2,
                    (OthelloGrid.RowDefinitions[0].Height.Value - OthelloTokenSize) / 2));

            //Check if the flip list contains tokens to flip, the conduct an animation
            if (oFlipList.Find(a => a.X == x &&
                                    a.Y == y) != null)
            {
                IsAnimate = true;
            }

            RenderOthelloToken(ob.Token, topLeft, OthelloTokenSize, OthelloTokenSize, IsAnimate);
        }

        /// Render an othello token with specified size in Screen Coordinates
        private void RenderOthelloToken(OthelloBitType oc, Point coordinate, double width, double height, bool IsAnimate = false)
        {
            Shape shapeToRender = null;

            switch (oc)
            {
                case OthelloBitType.Black:
                    shapeToRender = new Ellipse() { Stroke = Brushes.Gray, Fill = Brushes.Black, Width = width, Height = height };
                    break;
                case OthelloBitType.White:
                    shapeToRender = new Ellipse() { Stroke = Brushes.Gray, Fill = Brushes.White, Width = width, Height = height };
                    break;
                case OthelloBitType.Empty:
                    break;
                default:
                    break;
            }

            if (shapeToRender != null)
            {
                //need to get a point from screen coordinates to client (canvas) coordinates
                Point oLeft = CanvasDrawingArea.PointFromScreen(coordinate);

                Canvas.SetLeft(shapeToRender, oLeft.X);
                Canvas.SetTop(shapeToRender, oLeft.Y);

                int index = CanvasDrawingArea.Children.Add(shapeToRender);

                //start animating
                if (IsAnimate)
                {
                    IsAnimating.Enqueue(true);
                    DoubleAnimation dblAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1000));
                    dblAnim.Completed += dblAnim_Completed;
                    CanvasDrawingArea.Children[index].BeginAnimation(Shape.OpacityProperty, dblAnim);
                }

            }
        }

        //renderer meta data information like Player score
        private void RenderGameMetaData()
        {
            //update status text
            StatusText1.Text = OthelloGame.GameGetPlayerWhite().PlayerName + " Score: " + OthelloGame.GameGetScore(OthelloGame.GameGetPlayerWhite());
            StatusText2.Text = OthelloGame.GameGetPlayerBlack().PlayerName + " Score: " + OthelloGame.GameGetScore(OthelloGame.GameGetPlayerBlack());
            StatusText3.Text = string.Format("Turn = {0}", oTurn);
            StatusText4.Text = string.Format("Current Player = {0}", oCurrentPlayer);
            StatusText5.Text = oIsInvalidMove ? oInvalidMove : "";
            StatusText6.Text = OthelloGame.GameIsEndGame() ? "Game Ended!" : "";

            //update message box text
            MessageBox.AppendText(string.Format("Turn = {0}", oTurn) + "\n");
            MessageBox.AppendText(string.Format("Current Player = {0}", oCurrentPlayer) + "\n");

            //render othello token in status bar
            RenderOthelloToken(OthelloGame.GameGetPlayerWhite().GetPlayerOthelloToken(), PlayerACanvas.PointToScreen(new Point(2, 2)), 10, 10);
            RenderOthelloToken(OthelloGame.GameGetPlayerBlack().GetPlayerOthelloToken(), PlayerBCanvas.PointToScreen(new Point(2, 2)), 10, 10);
        }

        //TODO make beautiful UI
        private TextBlock RenderText(Point coordinate, string text, Color color)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Foreground = new SolidColorBrush(color);

            //need to get a point from screen coordinates to client (canvas) coordinates
            Canvas.SetLeft(tb, coordinate.X);
            Canvas.SetTop(tb, coordinate.Y);

            CanvasDrawingArea.Children.Add(tb);

            return tb;
        }


        #endregion

        #region AI HANDLERS
        /// <summary>
        /// Completed Event Handler (For A.I computation)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void dblAnim_Completed(object sender, EventArgs e)
        {
            //Trace.WriteLine("animation complete");

            IsAnimating.Dequeue();

            if (IsAnimating.Count == 0)
            {
                //Computer moves after human moves animation
                //if (oGame.GameMode == GameMode.HumanVSComputer && 
                //    !oGame.GameIsEndGame() &&
                //    oGame.GameUpdatePlayer().PlayerKind == oGame.AIPlayer.PlayerKind)
                if(OthelloGame.GameGetMode() == GameMode.HumanVSComputer &&
                    !OthelloGame.GameIsEndGame() &&
                    OthelloGame.GameUpdatePlayer().PlayerKind == OthelloGame.GameGetAiPlayer().AiPlayer.PlayerKind)
                {
                    var slowTask = Task.Factory.StartNew(() =>ComputerMoves());

                    //disable all cell buttons when computing
                    SetEnableGridButtons(false);
                    //disable all UI buttons when computing
                    SetUIButton(false);

                    //start a progress bar in case the computation for ComputerMoves is very slow
                    DoubleAnimation dblAnimProgress = new DoubleAnimation(0, 100, TimeSpan.FromMilliseconds(30000));
                    ComputeProgressBar.BeginAnimation(ProgressBar.ValueProperty, dblAnimProgress);

                    await slowTask;

                    //stop the animation
                    ComputeProgressBar.BeginAnimation(ProgressBar.ValueProperty, null);

                    //enable all buttons again
                    SetEnableGridButtons(true);
                    SetUIButton(true);

                    Refresh();
                }
            }
        }

        private void ComputerMoves()
        {          
            //oFlipList = oGame.GameAIMakeMove();
            oFlipList = OthelloGame.GameAIMakeMove();

            //TODO : this statement necessary?
            //oIsInvalidMove = oFlipList.Count() < 1 ? true : false;
        }

        #endregion

        #region UI SETTERS
        /// <summary>
        /// Set the cell buttons to disable/enable after each placement
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetOthelloUXCellAccess(int x, int y)
        {
            //MEMO: to set disabled style of button programmatically, use a Custom Control Template (in xaml)

            //get an item from the grid
            var item = LogicalTreeHelper
                            .GetChildren(OthelloGrid)
                            .OfType<Button>()
                            .Select(brd => brd)
                            .Where(a => Grid.GetColumn(a) == x && Grid.GetRow(a) == y).Single();

            switch(oBoard[x,y].Token)
            {
                case OthelloBitType.White:
                case OthelloBitType.Black:
                    item.IsEnabled = false;                   
                    break;
                case OthelloBitType.Empty:
                    item.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }

        private void SetEnableGridButtons(bool enable)
        {
            //get an item from the grid
            var items = LogicalTreeHelper
                            .GetChildren(OthelloGrid)
                            .OfType<Button>()
                            .Select(brd => brd);

            foreach(Button b in items)
            {
                b.IsEnabled = enable;
            }

        }

        private void SetUIButton(bool enable)
        {
            Load.IsEnabled = enable;
            Save.IsEnabled = enable;
            NewGame.IsEnabled = enable;
            Undo.IsEnabled = enable;
            Redo.IsEnabled = enable;
            NewGame_AI.IsEnabled = enable;
            HumanWhiteCheck.IsEnabled = enable;
            Difficulty_ComboBox.IsEnabled = enable;
        }

        #endregion

        private void MessageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MessageBox.CaretIndex = MessageBox.Text.Length;
            MessageBox.ScrollToEnd();
        }
    }
}
