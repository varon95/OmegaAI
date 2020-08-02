using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{

    public partial class Form1 : Form
    {
        //negaMax
        int depth = 1;
        int testdepth = 1;
        string evaluationFunctionName = "evaluationFunction";
        int evalTime = 0;

        //button size
        public int w = 40;
        public int h = 60;
        public int margin = 5;

        //grid size
        public int n = 4;

        //grid settings
        public int x0;
        public int x;
        public int y;
        public int buttonID = 1;

        //player
        public bool player = true;
        public int countSteps = 1;
        public int stepsLeft;


        //lists
        // fieldList is currently not in use
        public List<Field> fieldList = new List<Field>();

        // contains group id's, group values, and a list of group members
        public List<group> groups = new List<group>();

        //board array - contains all the fields on the board
        public Field[,] board;

        //results
        public int resultBlacks;
        public int resultWhites;

        /* 
         array for Union Find
         0.: id
         1.: root (parent)
         2.: old root (old parent)
         3.: size
        */
        public byte[,] searchTree;

        // storing root nodes/sizes and previous root nodes/sizes for blacks
        public List<Root> listBlacks = new List<Root>();
        public List<Root> previousListBlacks = new List<Root>();

        // storing root nodes/sizes and previous root nodes/sizes for whites
        public List<Root> listWhites = new List<Root>();
        public List<Root> previousListWhites = new List<Root>();

        // move order
        public List<byte> moves = new List<byte>();
        

        public Form1()
        {
            InitializeComponent();
        }

        //calculating the game score
        public void Score(List<Root> listWhites, List<Root> listBlacks, out int scoreWhites, out int scoreBlacks)
        {
            scoreWhites = 1;
            scoreBlacks = 1;
            foreach (var item in listWhites)
            {
                scoreWhites = scoreWhites * item.RootSize;
            }
            foreach (var item in listBlacks)
            {
                scoreBlacks = scoreBlacks * item.RootSize;
            }
            return;
        }

        // find algorithm, finds if the root of a field
        // path compressing is optional
        public void Find(byte p, bool pathCompressing, out byte root)
        {
            root = p;
            while (searchTree[root,1]!= root)
            {
                root = searchTree[root, 1];

                if (pathCompressing==true)
                {
                    while (p!=root)
                    {
                        byte next = searchTree[p, 1];
                        searchTree[p, 1] = root;
                        p = next;
                    }
                }
            }
            return;
        }

        // connected - checks if the two components are in the same group
        public void Connected(byte p, byte q, bool pathCompressing, out bool connected)
        {
            Find(p, pathCompressing, out byte rootP);
            Find(q, pathCompressing, out byte rootQ);
            connected = (rootP == rootQ);
            return;
        }
        
        // unifyInGame unites groups smaller to larger, uses path compression
        // does not store previous parent
        public void unifyInGame (byte p, byte q, List<Root> roots)
        {
            Find(p, true, out byte root1);
            Find(q, true, out byte root2);

            if (root1==root2) { return; }

            if (searchTree[root1,3] < searchTree[root1, 3]) //root 2 is larger
            {
                //remove old root
                roots.Remove(roots.Find(item => item.RootId == root1));
                //set root2 as parent
                searchTree[root1, 1] = root2;
                //set size
                searchTree[root2, 3] = (byte)(searchTree[root2, 3] + searchTree[root1, 3]);
                //set new roots in list
                roots.Remove(roots.Find(item => item.RootId == root2));
                
                Root newRoot = new Root();
                newRoot.RootId = root1;
                newRoot.RootSize = searchTree[root2, 3];
                roots.Add(newRoot);
                

            }
            else
            {
                //remove old root
                roots.Remove(roots.Find(item => item.RootId == root2));
                //set root1 as parent
                searchTree[root2, 1] = root1;
                //set size
                searchTree[root1, 3] = (byte)(searchTree[root2, 3] + searchTree[root1, 3]);
                //set roots in list
                roots.Remove(roots.Find(item => item.RootId == root1));
                
                Root newRoot = new Root();
                newRoot.RootId = root1;
                newRoot.RootSize = searchTree[root1, 3];
                roots.Add(newRoot);
                
                
            }
        }

        // unifyAB unites groups older to newer, no path compression
        // storing previous parent
        public void unifyAB(byte rootNew, byte oldQ, List<Root> newRoots, List<Root> oldRoots)
        {
            Find(oldQ, false, out byte rootOld);

            if (rootNew == rootOld) { return; }

            //set rootNew as parent
            searchTree[rootOld, 2] = searchTree[rootOld, 1];
            searchTree[rootOld, 1] = rootNew;
            //set size
            searchTree[rootNew, 3] = (byte)(searchTree[rootOld, 3] + searchTree[rootNew, 3]);

            //delete old root, and add it to old root list
            byte index = (byte)newRoots.FindIndex(item => item.RootId == rootOld);
            Root oldRoot = newRoots[index];
            oldRoot.removedBy = rootNew;
            oldRoots.Add(oldRoot);
            newRoots.RemoveAt(index);

            //remove new root from root list (if exists)
            index = (byte)(newRoots.Count - 1);
            try
            {
                if (newRoots[index].RootId == rootNew)
                {
                    newRoots.RemoveAt(index);
                }

            } catch (Exception) { }

            //adding new root to rootlist
            Root newRoot = new Root();
            newRoot.RootId = rootNew;
            newRoot.RootSize = searchTree[rootNew, 3];
            newRoots.Add(newRoot);
        }

        //Remove Last
        public void removeLast (byte toRemove, List<Root> newRoots, List<Root> oldRoots)
        {
            int index = newRoots.Count - 1;
            int lengthOldRoots = oldRoots.Count - 1;
            byte restoreID;
            // restore last element from oldRoots
            try
            {
                if (oldRoots[lengthOldRoots].removedBy == toRemove)
                {
                    Root a = oldRoots[lengthOldRoots];
                    restoreID = a.RootId;
                    a.removedBy = 0;
                    newRoots.Add(a);
                    oldRoots.RemoveAt(lengthOldRoots);
                    //restore search tree root <- old root; old root = 0
                    searchTree[restoreID, 1] = searchTree[restoreID, 2];
                    searchTree[restoreID, 2] = 0;


                    // restore 1 and 2 before last element from old roots
                    // if they were removed by to remove
                    try
                    {
                        if (oldRoots[lengthOldRoots - 1].removedBy == toRemove)
                        {
                            Root b = oldRoots[lengthOldRoots - 1];
                            restoreID = b.RootId;
                            b.removedBy = 0;
                            newRoots.Add(b);
                            oldRoots.RemoveAt(lengthOldRoots - 1);

                            //restore search tree root <- old root; old root = 0
                            searchTree[restoreID, 1] = searchTree[restoreID, 2];
                            searchTree[restoreID, 2] = 0;
                            try
                            {
                                if (oldRoots[lengthOldRoots - 2].removedBy == toRemove)
                                {
                                    Root c = oldRoots[lengthOldRoots - 2];
                                    restoreID = c.RootId;
                                    c.removedBy = 0;
                                    newRoots.Add(c);
                                    oldRoots.RemoveAt(lengthOldRoots - 2);

                                    //restore search tree root <- old root; old root = 0
                                    searchTree[restoreID, 1] = searchTree[restoreID, 2];
                                    searchTree[restoreID, 2] = 0;
                                }

                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            // restore group data for ex root
            // parent, old parent, size
            searchTree[toRemove, 1] = toRemove;
            searchTree[toRemove, 2] = 0;
            searchTree[toRemove, 3] = 1;

            //probably te removable element is in the lastt place
            //but sometimes its not...
            if (newRoots[index].RootId==toRemove)
            {
                newRoots.RemoveAt(index);
            }
            else
            {
                newRoots.Remove(newRoots.Find(item => item.RootId == toRemove));
            }
            
        }

        public void checkNeighbours(Field thisField, bool inGame, List<Root> list1, List<Root> list2 = null)
        {
            //coordinates of button
            int row = thisField.row;
            int col = thisField.column;
            //count of neighbours
            int neighbourCount = 0;


            //Search neighbours - check if neighbours.color == button.color
            try
            {
                if (thisField.color == board[row - 1, col - 1].color)
                {
                    neighbourCount = neighbourCount + 1;
                    //unify
                    if (inGame) { unifyInGame((byte)thisField.id, (byte)board[row - 1, col - 1].id, list1); }
                    else { unifyAB((byte)thisField.id, (byte)board[row - 1, col - 1].id, list1, list2); }
                }
            }
            catch (Exception) { }

            try
            {
                if (thisField.color == board[row - 1, col].color)
                {
                    neighbourCount = neighbourCount + 1;
                    //unify
                    if (inGame) { unifyInGame((byte)thisField.id, (byte)board[row - 1, col].id, list1); }
                    else { unifyAB((byte)thisField.id, (byte)board[row - 1, col].id, list1, list2); }
                }
            }
            catch (Exception) { }

            try
            {
                if (thisField.color == board[row, col - 1].color)
                {
                    neighbourCount = neighbourCount + 1;
                    //unify
                    if (inGame) { unifyInGame((byte)thisField.id, (byte)board[row, col - 1].id, list1); }
                    else { unifyAB((byte)thisField.id, (byte)board[row, col - 1].id, list1, list2); }
                }
            }
            catch (Exception) { }

            try
            {
                if (thisField.color == board[row, col + 1].color)
                {
                    neighbourCount = neighbourCount + 1;
                    //unify
                    if (inGame) { unifyInGame((byte)thisField.id, (byte)board[row, col + 1].id, list1); }
                    else
                    { unifyAB((byte)thisField.id, (byte)board[row, col + 1].id, list1, list2); }
                }
            }
            catch (Exception) { }

            try
            {
                if (thisField.color == board[row + 1, col].color)
                {
                    neighbourCount = neighbourCount + 1;
                    //unify
                    if (inGame) { unifyInGame((byte)thisField.id, (byte)board[row + 1, col].id, list1); }
                    else { unifyAB((byte)thisField.id, (byte)board[row + 1, col].id, list1, list2); }
                }
            }
            catch (Exception) { }

            try
            {
                if (thisField.color == board[row + 1, col + 1].color)
                {
                    neighbourCount = neighbourCount + 1;
                    //unify
                    if (inGame) { unifyInGame((byte)thisField.id, (byte)board[row + 1, col + 1].id, list1); }
                    else { unifyAB((byte)thisField.id, (byte)board[row + 1, col + 1].id, list1, list2); }
                }
            }
            catch (Exception) { }

            if (neighbourCount == 0)
            {
                //setting search tree values
                //adding new root
                Root newRoot = new Root();
                newRoot.RootId = (byte)thisField.id;
                newRoot.RootSize = 1;
                list1.Add(newRoot);

            }
        }



        // Negamax with alpha-beta
        /*
            function negamax(node, depth, α, β, color) is
                if depth = 0 or node is a terminal node then
                    return color × the heuristic value of node

                childNodes := generateMoves(node)
                childNodes := orderMoves(childNodes)
                value := −∞
                foreach child in childNodes do
                    value := max(value, −negamax(child, depth − 1, −β, −α, −color))
                    α := max(α, value)
                    if α ≥ β then
                        break (* cut-off *)
                return value
        */




        public void NegaMax(List<group> groups , List<Field> boardList, int stepsLeft, int depth, int alpha, int beta, int color,
            out int value, out Field stepWithWhite, out Field stepWithBlack, string evaluationName = "evaluationFunctionbyScore")
        {
            Field bestWhite = new Field();
            Field bestBlack = new Field();

            if (depth==0 || stepsLeft==0)
            {
                //evaluate boardList
                //default
                if (evaluationName == "evaluationFunctionbyScore") { value = evaluationFunctionbyScore(color); }
                // if n = 5
                else if (evaluationName == "evaluationPhase1") { value = evaluationPhase1(color); }
                else if (evaluationName == "evaluationPhase2") { value = evaluationPhase2(color); }
                else { value = evaluationPhase3(color); }

                stepWithWhite = bestWhite;
                stepWithBlack = bestBlack;
                return;

            }


            value = -1000000;
            //bestWhite = new Field();
            //bestBlack = new Field();
            int dummy;

            // generating move for white stones
            List<Field> emptyWhites = new List<Field>();
            
            foreach (var field in boardList)
            {
                if (field.color==0)
                {
                    emptyWhites.Add(field);                   
                }
                
            }

            //are there any mustDo moves?
            bool mustDoWithWhite = false;
            bool mustDoWithBlack = false;
            //field for mustDo move
            Field mustDoMove = new Field();
            //mustDo move check part1
            if (n==5 && depth == testdepth && evaluationFunctionName == "evaluationPhase3")
            {
                //check for mustDo moves


                  //here comes the checking part

                //get id of largest and second largest groupp of white
                    int bigestWhite = 0;
                    int secondBigestWhite = 0;
                    int size = 0;
                    foreach (var white in listWhites)
                    {
                        if (white.RootSize >= size)
                        {
                            secondBigestWhite = bigestWhite;
                            bigestWhite = white.RootId;
                        }
                    }

                   //get id of largest and second largest groupp of white
                    int bigestBlack = 0;
                    int secondBigestBlack = 0;
                    size = 0;
                    foreach (var black in listBlacks)
                    {
                        if (black.RootSize >= size)
                        {
                            secondBigestWhite = bigestWhite;
                            bigestWhite = black.RootId;
                        }
                    }

                    foreach (Field white in emptyWhites)
                    {
                        int row = white.row;
                        int col = white.column;

                        byte rootN1;
                        byte rootN2;
                        byte rootN3;
                        byte rootN4;
                        byte rootN5;
                        byte rootN6;

                        try {Find((byte)board[row - 1, col - 1] .id, false, out rootN1); } catch (Exception){ rootN1 = 0; }
                        try {Find((byte)board[row - 1, col]     .id, false, out rootN2); } catch (Exception){ rootN2 = 0; }
                        try {Find((byte)board[row, col - 1]     .id, false, out rootN3); } catch (Exception){ rootN3 = 0; }
                        try {Find((byte)board[row, col + 1]     .id, false, out rootN4); } catch (Exception){ rootN4 = 0; }
                        try {Find((byte)board[row + 1, col]     .id, false, out rootN5); } catch (Exception){ rootN5 = 0; }
                        try {Find((byte)board[row + 1, col + 1] .id, false, out rootN6); } catch (Exception){ rootN6 = 0; }

                        List<byte> neighboursList = new List<byte>();
                        neighboursList.Add(rootN1);
                        neighboursList.Add(rootN2);
                        neighboursList.Add(rootN3);
                        neighboursList.Add(rootN4);
                        neighboursList.Add(rootN5);
                        neighboursList.Add(rootN6);

                        foreach (byte item in neighboursList)
                        {
                            if (item == bigestWhite)
                            {
                                foreach (byte item2 in neighboursList)
                                {
                                    if (item2 == secondBigestWhite) // mustDo moove found
                                    {
                                    // if it is blackAI -> connect the stones using whites
                                    if (color == -1)
                                    {
                                        mustDoWithWhite = true;
                                        mustDoMove = white;
                                    }

                                    // it it is whiteAI -> separate using blacks
                                    else
                                    {
                                        mustDoWithBlack = true;
                                        mustDoMove = white;
                                    }

                                }
                                }
                            }

                            if (item == bigestBlack)
                            {
                                foreach (byte item2 in neighboursList)
                                {
                                    if (item2 == secondBigestBlack) // mustDo moove found
                                    {
                                    //if it is blackAI -> separate using whites
                                    if (color == -1)
                                    {
                                        mustDoWithWhite = true;
                                        mustDoMove = white;
                                    }

                                    // it it is whiteAI -> connect the stones using blacks
                                    else
                                    {
                                        mustDoWithBlack = true;
                                        mustDoMove = white;
                                    }
                                }
                                }
                            }
                        }
                    }


            }
            if (mustDoWithWhite)
            {
                // set white to mustDo move
                Field white = mustDoMove;
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                List < Field > emptyBlacks = new List<Field>();
                foreach (var empty in emptyWhites)
                {
                    emptyBlacks.Add(empty);
                }
                emptyBlacks.Remove(white);


                //adding whites
                white.color = 1;

                checkNeighbours(white, false, listWhites, previousListWhites);

                foreach (var black in emptyBlacks)
                {
                    //adding blacks
                    black.color = 2;

                    /* search tree loop check
                    for (int i = 0; i < (n * (2 * n - 1) + (int)Math.Pow((n - 1), 2)) + 1; i++)
                    {
                        if (searchTree[searchTree[i, 1], 1] == searchTree[i, 0] && searchTree[i, 1] != i)
                        {
                            MessageBox.Show("bukóó");
                        }

                    }
                    */
                        checkNeighbours(black, false, listBlacks, previousListBlacks);

                    NegaMax(groups, boardList, stepsLeft - 2, depth - 1, -beta, -alpha, -color, out dummy, out Field d1, out Field d2);

                    //setting bestStep
                    if (-dummy > value)
                    {
                        bestWhite = white;
                        bestBlack = black;

                        value = -dummy;

                    }

                    //value = Math.Max(value, -dummy);
                    alpha = Math.Max(value, alpha);

                    if (alpha >= beta)
                    {
                        black.color = 0;
                        removeLast((byte)black.id, listBlacks, previousListBlacks);

                        break; // cut-off
                    }

                    //removing blacks
                    black.color = 0;

                    removeLast((byte)black.id, listBlacks, previousListBlacks);

                }

                //removig whites
                white.color = 0;

                removeLast((byte)white.id, listWhites, previousListWhites);
            }       //if mustDo move with white is possible
            else if (mustDoWithBlack)
            {
                // set white to mustDo move
                Field white = mustDoMove;
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                //here comes the tricky part...
                //lets pretend that black is white;)
                List<Field> emptyBlacks = new List<Field>();
                foreach (var empty in emptyWhites)
                {
                    emptyBlacks.Add(empty);
                }
                emptyBlacks.Remove(white);


                //adding whites
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! this is black -> 2
                white.color = 2;

                checkNeighbours(white, false, listBlacks, previousListBlacks);

                foreach (var black in emptyBlacks)
                {
                    //adding blacks
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! this is white -> 2
                    black.color = 1;

                    /* search tree loop check
                    for (int i = 0; i < (n * (2 * n - 1) + (int)Math.Pow((n - 1), 2)) + 1; i++)
                    {
                        if (searchTree[searchTree[i, 1], 1] == searchTree[i, 0] && searchTree[i, 1] != i)
                        {
                            MessageBox.Show("bukóó");
                        }

                    }
                    */
                    checkNeighbours(black, false, listWhites, previousListWhites);

                    NegaMax(groups, boardList, stepsLeft - 2, depth - 1, -beta, -alpha, -color, out dummy, out Field d1, out Field d2);

                    //setting bestStep
                    if (-dummy > value)
                    {
                        // its exciting, im am sure im gonna miss sth
                        // black is white, white is black
                        bestWhite = black;
                        bestBlack = white;

                        value = -dummy;

                    }

                    //value = Math.Max(value, -dummy);
                    alpha = Math.Max(value, alpha);

                    if (alpha >= beta)
                    {
                        black.color = 0;

                        //again, beware black is white
                        removeLast((byte)black.id, listWhites, previousListWhites);

                        break; // cut-off
                    }

                    //removing blacks
                    black.color = 0;

                    //again, beware white is black
                    removeLast((byte)black.id, listWhites, previousListWhites);

                }

                //removig whites
                white.color = 0;

                //again, beware white is black
                removeLast((byte)white.id, listBlacks, previousListBlacks);
            } //if mustDo move with black is possible
            
            else
            {
                foreach (var white in emptyWhites)
                {

                    // generating move for black stones
                    List<Field> emptyBlacks = new List<Field>();
                    foreach (var empty in emptyWhites)
                    {
                        emptyBlacks.Add(empty);
                    }
                    emptyBlacks.Remove(white);


                    //adding whites
                    white.color = 1;

                    checkNeighbours(white, false, listWhites, previousListWhites);

                    foreach (var black in emptyBlacks)
                    {
                        //adding blacks
                        black.color = 2;

                        /* search tree loop check
                        for (int i = 0; i < (n * (2 * n - 1) + (int)Math.Pow((n - 1), 2)) + 1; i++)
                        {
                            if (searchTree[searchTree[i, 1], 1] == searchTree[i, 0] && searchTree[i, 1] != i)
                            {
                                MessageBox.Show("bukóó");
                            }

                        }
                        */
                        checkNeighbours(black, false, listBlacks, previousListBlacks);

                        NegaMax(groups, boardList, stepsLeft - 2, depth - 1, -beta, -alpha, -color, out dummy, out Field d1, out Field d2, evaluationName);

                        //setting bestStep
                        if (-dummy > value)
                        {
                            bestWhite = white;
                            bestBlack = black;

                            value = -dummy;

                        }

                        //value = Math.Max(value, -dummy);
                        alpha = Math.Max(value, alpha);

                        if (alpha >= beta)
                        {
                            black.color = 0;
                            removeLast((byte)black.id, listBlacks, previousListBlacks);

                            break; // cut-off
                        }

                        //removing blacks
                        black.color = 0;

                        removeLast((byte)black.id, listBlacks, previousListBlacks);

                    }

                    //removig whites
                    white.color = 0;

                    removeLast((byte)white.id, listWhites, previousListWhites);

                }
            }

            stepWithWhite = bestWhite;
            stepWithBlack = bestBlack;
            return;
        }

        private int evaluationFunctionbyScore(int color)
        {
            int value;
            Score(listWhites, listBlacks, out int evalW, out int evalB);
            value = color * (evalW - evalB);
            return value;
        }

        /* 
            phase1 (round 1-2):

            lower points to center, higher points to corners
            + points for groups of 1
            dont get more than 3 groups
            "manhattan distance" maximization
         */
        private int evaluationPhase1(int color)
        {
            int value;
            //parameters
            //score for group sizes
            int scoreForGroupOf1 = 7;
            int scoreForGroupOf2 = 4;
            int scoreForGroupOf3 = 1;

            int[] positionScore = new int[62];
            positionScore[1] = 6;
            positionScore[2] = 5;
            positionScore[3] = 5;
            positionScore[4] = 5;
            positionScore[5] = 6;
            positionScore[6] = 5;
            positionScore[7] = 4;
            positionScore[8] = 4;
            positionScore[9] = 4;
            positionScore[10] = 4;
            positionScore[11] = 5;
            positionScore[12] = 5;
            positionScore[13] = 4;
            positionScore[14] = 3;
            positionScore[15] = 3;
            positionScore[16] = 3;
            positionScore[17] = 4;
            positionScore[18] = 5;
            positionScore[19] = 5;
            positionScore[20] = 4;
            positionScore[21] = 3;
            positionScore[22] = 2;
            positionScore[23] = 2;
            positionScore[24] = 3;
            positionScore[25] = 4;
            positionScore[26] = 5;
            positionScore[27] = 6;
            positionScore[28] = 4;
            positionScore[29] = 3;
            positionScore[30] = 2;
            positionScore[31] = 1;
            positionScore[32] = 2;
            positionScore[33] = 3;
            positionScore[34] = 4;
            positionScore[35] = 6;
            positionScore[36] = 5;
            positionScore[37] = 4;
            positionScore[38] = 3;
            positionScore[39] = 2;
            positionScore[40] = 2;
            positionScore[41] = 3;
            positionScore[42] = 4;
            positionScore[43] = 5;
            positionScore[44] = 5;
            positionScore[45] = 4;
            positionScore[46] = 3;
            positionScore[47] = 3;
            positionScore[48] = 3;
            positionScore[49] = 4;
            positionScore[50] = 5;
            positionScore[51] = 5;
            positionScore[52] = 4;
            positionScore[53] = 4;
            positionScore[54] = 4;
            positionScore[55] = 4;
            positionScore[56] = 5;
            positionScore[57] = 6;
            positionScore[58] = 5;
            positionScore[59] = 5;
            positionScore[60] = 5;
            positionScore[61] = 6;


            // calculating points
          //  int scoreWhites = 1;
          //  int scoreBlacks = 1;
            int scoreGroupSizeWhites = 0;
            int scoreGroupSizeBlacks = 0;
            foreach (var item in listWhites)
            {
               // scoreWhites = scoreWhites * item.RootSize;

                if (item.RootSize == 1)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf1;
                }
                else if (item.RootSize == 2)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf2;
                }
                else if (item.RootSize == 3)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf3;
                }
            }
            foreach (var item in listBlacks)
            {
                //scoreBlacks = scoreBlacks * item.RootSize;

                if (item.RootSize == 1)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf1;
                }
                else if (item.RootSize == 2)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf2;
                }
                else if (item.RootSize == 3)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf3;
                }
            }

            //calculating score from board position
            //this only works with depth 1 search
            int blackScoreFromPosition = positionScore[listBlacks[listBlacks.Count - 1].RootId];
            int whiteScoreFromPosition = positionScore[listWhites[listWhites.Count - 1].RootId];

            int a = 1; //group size score
            int b = 3; //position score
           // int c = 1; //distance score

            value = color * (a*(scoreGroupSizeWhites - scoreGroupSizeBlacks) + b*(whiteScoreFromPosition - blackScoreFromPosition));

            return value;
        }

        /*
         lower points to center, higher points to sides
        negative points for groups of 1
        dont get more than 3 groups
        score
         */
        private int evaluationPhase2(int color)
        {
            int value;
            //parameters
            //score for group sizes
            int scoreForGroupOf1 = 0;
            int scoreForGroupOf2 = 4;
            int scoreForGroupOf3 = 8;
            int scoreForGroupOf4 = 8;
            int scoreForGroupOf5 = 8;
            int scoreForGroupOf6 = 4;
            int scoreForGroupOf7 = 3;
            int scoreForGroupOf8 = 1;

            int[] positionScore = new int[62];
            positionScore[1] = 6;
            positionScore[2] = 5;
            positionScore[3] = 5;
            positionScore[4] = 5;
            positionScore[5] = 6;
            positionScore[6] = 5;
            positionScore[7] = 4;
            positionScore[8] = 4;
            positionScore[9] = 4;
            positionScore[10] = 4;
            positionScore[11] = 5;
            positionScore[12] = 5;
            positionScore[13] = 4;
            positionScore[14] = 3;
            positionScore[15] = 3;
            positionScore[16] = 3;
            positionScore[17] = 4;
            positionScore[18] = 5;
            positionScore[19] = 5;
            positionScore[20] = 4;
            positionScore[21] = 3;
            positionScore[22] = 2;
            positionScore[23] = 2;
            positionScore[24] = 3;
            positionScore[25] = 4;
            positionScore[26] = 5;
            positionScore[27] = 6;
            positionScore[28] = 4;
            positionScore[29] = 3;
            positionScore[30] = 2;
            positionScore[31] = 1;
            positionScore[32] = 2;
            positionScore[33] = 3;
            positionScore[34] = 4;
            positionScore[35] = 6;
            positionScore[36] = 5;
            positionScore[37] = 4;
            positionScore[38] = 3;
            positionScore[39] = 2;
            positionScore[40] = 2;
            positionScore[41] = 3;
            positionScore[42] = 4;
            positionScore[43] = 5;
            positionScore[44] = 5;
            positionScore[45] = 4;
            positionScore[46] = 3;
            positionScore[47] = 3;
            positionScore[48] = 3;
            positionScore[49] = 4;
            positionScore[50] = 5;
            positionScore[51] = 5;
            positionScore[52] = 4;
            positionScore[53] = 4;
            positionScore[54] = 4;
            positionScore[55] = 4;
            positionScore[56] = 5;
            positionScore[57] = 6;
            positionScore[58] = 5;
            positionScore[59] = 5;
            positionScore[60] = 5;
            positionScore[61] = 6;

            //number of groups
            int numberOfWhiteGroups = 0;
            int numberOfBlackGroups = 0;

            // calculating points
            int scoreWhites = 1;
            int scoreBlacks = 1;
            int scoreGroupSizeWhites = 0;
            int scoreGroupSizeBlacks = 0;
            foreach (var item in listWhites)
            {
                scoreWhites = scoreWhites * item.RootSize;
                numberOfWhiteGroups = numberOfWhiteGroups + 1;

                if (item.RootSize == 1)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf1;
                }
                else if (item.RootSize == 2)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf2;
                }
                else if (item.RootSize == 3)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf3;
                }
                else if (item.RootSize == 4)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf4;
                }
                else if (item.RootSize == 5)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf5;
                }
                else if (item.RootSize == 6)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf6;
                }
                else if (item.RootSize == 7)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf7;
                }
                else if (item.RootSize == 8)
                {
                    scoreGroupSizeWhites = scoreGroupSizeWhites + scoreForGroupOf8;
                }
            }
            foreach (var item in listBlacks)
            {
                scoreBlacks = scoreBlacks * item.RootSize;

                numberOfBlackGroups = numberOfBlackGroups + 1;


                if (item.RootSize == 1)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf1;
                }
                else if (item.RootSize == 2)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf2;
                }
                else if (item.RootSize == 3)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf3;
                }
                else if (item.RootSize == 4)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf4;
                }
                else if (item.RootSize == 5)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf5;
                }
                else if (item.RootSize == 6)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf6;
                }
                else if (item.RootSize == 7)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf7;
                }
                else if (item.RootSize == 8)
                {
                    scoreGroupSizeBlacks = scoreGroupSizeBlacks + scoreForGroupOf8;
                }
            }

            //calculating score from board position
            //this only works with depth 1 search
            int blackScoreFromPosition = positionScore[listBlacks[listBlacks.Count - 1].RootId];
            int whiteScoreFromPosition = positionScore[listWhites[listWhites.Count - 1].RootId];

            int numberOfWhiteGroupsScore = 0;
            int numberOfBlackGroupsScore = 0;
            //goal is to get 3 - 4 individual groups
            // whites
            if (numberOfWhiteGroups == 3)
            {
                numberOfWhiteGroupsScore = 10;
            }
            if (numberOfWhiteGroups == 4)
            {
                numberOfWhiteGroupsScore = 8;
            }
            if (numberOfWhiteGroups == 5)
            {
                numberOfWhiteGroupsScore = 1;
            }
            // Blacks
            if (numberOfBlackGroups == 3)
            {
                numberOfBlackGroupsScore = 10;
            }
            if (numberOfBlackGroups == 4)
            {
                numberOfBlackGroupsScore = 8;
            }
            if (numberOfBlackGroups == 5)
            {
                numberOfBlackGroupsScore = 1;
            }

            int a = 1; //group size score
            int b = 1; //position score
            int c = 1; //"game score" score
            int d = 1; // number of groups score

            value = color * (a * (scoreGroupSizeWhites - scoreGroupSizeBlacks) + b * (whiteScoreFromPosition - blackScoreFromPosition)
                + c * (int)(Math.Sqrt(scoreWhites) - Math.Sqrt(scoreBlacks)) + d * (numberOfWhiteGroupsScore - numberOfBlackGroupsScore));
            
            return value;
        }

        // evaluation for phase 3 and the endgame
        private int evaluationPhase3(int color)
        {
             int scoreWhites = 1;
             int scoreBlacks = 1;
            foreach (var item in listWhites)
            {
              scoreWhites = scoreWhites * item.RootSize;
            }
            foreach (var item in listBlacks)
            {
              scoreBlacks = scoreBlacks * item.RootSize;
            }
            int value = color * ((int)(scoreWhites - scoreBlacks));

            return value;
        }




            // START THE GAME BUTTON
            private void startTheGame_Click(object sender, EventArgs e)
        {
         bool whiteAIChecked = whiteAI.Checked;
         bool blackAIChecked = blackAI.Checked;
         bool twoPlayersChecked = twoPlayers.Checked;

         // storing root nodes/sizes and previous root nodes/sizes for blacks
         listBlacks = new List<Root>();
         previousListBlacks = new List<Root>();

         // storing root nodes/sizes and previous root nodes/sizes for whites
         listWhites = new List<Root>();
         previousListWhites = new List<Root>();

         //move order
         moves = new List<byte>();

            panel1.Controls.Clear();
            groups.Clear();

            fieldList.Clear();
            player = true;

            whiteScore.Text = "-";
            blackScore.Text = "-";

            radioButtonWhite.Checked = true;
            countSteps = 1;
            buttonID = 1;

            try
            {
                n = int.Parse(nNumber.Text);
                
                //declaring board array
                board = new Field[2 * n - 1, 2 * n - 1];
                //declaring search tree
                searchTree = new byte[(n * (2 * n - 1) + (int)Math.Pow((n - 1), 2)) + 1, 4];
            }
            catch (Exception)
            {
                throw;
            }
            // steps counting
            int totalSteps = (n * (2 * n - 1) + (int)Math.Pow((n - 1), 2));
            int leftOver = totalSteps % 4;
            stepsLeft = totalSteps - leftOver;

            stepsLeftLabel.Text = stepsLeft.ToString();
            stepsLabel.Text = stepsLeft.ToString();

            for (int i = 0; i < 2 * n - 1; i++)
            {
                y = i * h * 2 / 3;
                x0 = Math.Abs(n - 1 - i) * w / 2;

                for (int j = 0; j < (2 * n - 1) - Math.Abs(n - 1 - i); j++)
                {
                    x = x0 + j * w;


                    Field dynamicButton = new Field();

                    // Define the points in the polygonal path.
                    Point[] pts = {
                                    new Point( w/2+margin,  0+margin),
                                    new Point(w+margin,  h/3+margin),
                                    new Point(w+margin,  h/3*2+margin),
                                    new Point(w/2+margin, h+margin),
                                    new Point(0+margin, h/3*2+margin),
                                    new Point(0+margin, h/3+margin)

                                };


                    // Make the GraphicsPath.
                    GraphicsPath polygon_path = new GraphicsPath(FillMode.Winding);
                    polygon_path.AddPolygon(pts);

                    // Convert the GraphicsPath into a Region.
                    Region polygon_region = new Region(polygon_path);

                    // Constrain the button to the region.
                    dynamicButton.Region = polygon_region;

                    // Make the button big enough to hold the whole region.
                    dynamicButton.SetBounds(
                        dynamicButton.Location.X,
                        dynamicButton.Location.Y,
                        40 + 2 * margin, 60 + 2 * margin);

                    dynamicButton.Left = dynamicButton.Location.X + x + j;
                    dynamicButton.Top = dynamicButton.Location.Y + y + i;


                    dynamicButton.id = buttonID;
                    // for evaluation purposes
                    dynamicButton.originalID = buttonID;
                    dynamicButton.row = i;
                    dynamicButton.color = 0;

                    //setting search tree!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    /* 
                     array for Union Find
                     0.: id
                     1.: root (parent)
                     2.: old root (old parent)
                     3.: size
                    */
                    searchTree[buttonID, 0] = (byte)(buttonID);
                    searchTree[buttonID, 1] = (byte)(buttonID);
                    searchTree[buttonID, 2] = 0;
                    searchTree[buttonID, 3] = 1;

                    if ((n - 1 - i) >= 0)
                    {
                        dynamicButton.column = j;
                        //add button to board
                        board[i, j] = dynamicButton;
                    }
                    else
                    {
                        dynamicButton.column = Math.Abs(n - 1 - i) + j;
                        //add button to board
                        board[i, Math.Abs(n - 1 - i) + j] = dynamicButton;
                    }

                    int a = dynamicButton.column;
                    dynamicButton.Text = dynamicButton.row.ToString() + " , " + dynamicButton.column.ToString();
                    


                    panel1.Controls.Add(dynamicButton);
                    fieldList.Add(dynamicButton);

                    // --------------------- BE CAREFUL AARON ------------------
                    dynamicButton.Click += (object sender1, EventArgs args) =>
                    {
                        if (player == true)
                        {
                            dynamicButton.BackColor = Color.White;
                            dynamicButton.color = 1;
                            dynamicButton.Enabled = false;
                            //grouping
                           // checkNeighbours(dynamicButton, true, listWhites);

                            checkNeighbours(dynamicButton, false, listWhites, previousListWhites);
                            moves.Add((byte)dynamicButton.id);

                        }
                        else
                        {
                            dynamicButton.BackColor = Color.DarkGray;
                            dynamicButton.color = 2;
                            dynamicButton.Enabled = false;
                            //grouping
                           // checkNeighbours(dynamicButton, true, listBlacks);

                            checkNeighbours(dynamicButton, false, listBlacks, previousListBlacks);
                            moves.Add((byte)dynamicButton.id);
                        }
                        player = !player;

                        //Calculate result
                        Score(listWhites, listBlacks, out resultWhites, out resultBlacks);


                        whiteScore.Text = resultWhites.ToString();
                        blackScore.Text = resultBlacks.ToString();

                        //steps left count
                        stepsLeft--;
                        stepsLeftLabel.Text = stepsLeft.ToString();
                        countSteps++;
                        // Game end check
                        if (stepsLeft == 0) { MessageBox.Show("The game has ended"); return; }

                        /*
                        whites.Clear();
                        blacks.Clear();
                        */

                        // radio button control
                        if (countSteps % 4 == 2 || countSteps % 4 == 1)
                        {
                            radioButtonWhite.Checked = true;
                            radioButtonBlack.Checked = false;

                            // white AI plays
                            if (whiteAIChecked)
                            {
                                //
                                if (n == 5)
                                {
                                    if (countSteps < 4) //phase 1
                                    {
                                        evaluationFunctionName = "evaluationPhase1";
                                        NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, 1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase1");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                    }
                                    else if (countSteps < 20) //phase 2
                                    {
                                        evaluationFunctionName = "evaluationPhase2";

                                        NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, 1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase2");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                    }
                                    else if (countSteps < 50) // phase 3
                                    {
                                        evaluationFunctionName = "evaluationPhase3";
                                        int fixDepth = 1;
                                        if (countSteps > 38)
                                        {
                                            fixDepth = 2;
                                        }

                                        // the code that you want to measure comes here
                                        NegaMax(groups, fieldList, stepsLeft - 1, fixDepth, -1000000, 1000000, 1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase3");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                     
                                    }
                                    else // endgame
                                    {
                                         evaluationFunctionName = "evaluationPhase4";
                                        int fixDepth = 3;

                                        // the code that you want to measure comes here
                                        NegaMax(groups, fieldList, stepsLeft - 1, fixDepth, -1000000, 1000000, 1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase3");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                                                                
                                    }
                                }
                                else
                                {
                                    panel1.Enabled = false;
                                    NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, 1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack);

                                    steppingFunction(value, stepWithWhite, stepWithBlack);
                                    //end check
                                }
                                if (stepsLeft == 0) { MessageBox.Show("The game has ended"); return; }

                                panel1.Enabled = true;
                            }
                        }
                        else // black player
                        {
                            radioButtonWhite.Checked = false;
                            radioButtonBlack.Checked = true;

                            // black AI plays
                            if (blackAIChecked)
                            {
                                panel1.Enabled = false;
                                if (n==5)
                                {
                                    if (countSteps < 4) //phase 1
                                    {
                                        evaluationFunctionName = "evaluationPhase1";
                                        NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, -1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase1");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                    }
                                    else if (countSteps < 20) //phase 2
                                    {
                                        evaluationFunctionName = "evaluationPhase2";
                                       
                                        NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, -1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase2");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                    }
                                    else if (countSteps < 50) //phase 3
	                                {
                                        evaluationFunctionName = "evaluationPhase3";
                                        int fixDepth = 1;
                                        if (countSteps > 38)
	                                    {
                                           fixDepth = 2;
	                                    } 
                                       
                                        NegaMax(groups, fieldList, stepsLeft - 1, fixDepth, -1000000, 1000000, -1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase2");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);
	                                }
                                    else // endgame
                                    {
                                        evaluationFunctionName = "evaluationPhase4";

                                        int fixDepth = 3;
                                        //var watch = System.Diagnostics.Stopwatch.StartNew();

                                        // the code that you want to measure comes here
                                        NegaMax(groups, fieldList, stepsLeft - 1, fixDepth, -1000000, 1000000, -1,
                                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                                        "evaluationPhase3");

                                        steppingFunction(value, stepWithWhite, stepWithBlack);

                                        //watch.Stop();
                                        //int elapsedSec = (int)(watch.ElapsedMilliseconds/1000);
                                        //if (elapsedSec<10)
                                        //{
                                        //    testdepth = testdepth + 1;
                                        //}
                                        //if (elapsedSec > 40)
                                        //{
                                        //    testdepth = testdepth - 1;
                                        //}
                                        //if (testdepth > 3)
                                        //{
                                        //    testdepth = 3;
                                        //}
                                    }
                                }
                                else
                                {
                                    NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, -1,
                                                                        out int value, out Field stepWithWhite, out Field stepWithBlack);

                                    steppingFunction(value, stepWithWhite, stepWithBlack);
                                }
                                
                                panel1.Enabled = true;
                                //end check
                                if (stepsLeft == 0) { MessageBox.Show("The game has ended"); return; }
                            }

                        }
                    };
                    buttonID++;
                }
            }

            // white AI first move
            if (whiteAIChecked)
            {
                panel1.Enabled = false;
                if (n == 5)
                { //phase 1

                        evaluationFunctionName = "evaluationPhase1";
                        NegaMax(groups, fieldList, stepsLeft - 1, depth, -1000000, 1000000, 1,
                        out int value, out Field stepWithWhite, out Field stepWithBlack,
                        "evaluationPhase1");

                        steppingFunction(value, stepWithWhite, stepWithBlack);
                }

                else
                {
                    NegaMax(groups, fieldList, stepsLeft, depth, -1000000, 1000000, 1,
                        out int value, out Field stepWithWhite, out Field stepWithBlack);

                    steppingFunction(value, stepWithWhite, stepWithBlack);

                }

                panel1.Enabled = true;
            }


        }

        private void steppingFunction(int value, Field stepWithWhite, Field stepWithBlack)
        {
            foreach (var item in fieldList)
            {
                if (item.id == stepWithWhite.id)
                {
                    item.color = 1;
                    item.BackColor = Color.White;
                    //grouping
                    // checkNeighbours(dynamicButton, true, listWhites);

                    checkNeighbours(item, false, listWhites, previousListWhites);
                    moves.Add((byte)item.id);
                    item.Enabled = false;

                }

                if (item.id == stepWithBlack.id)
                {
                    item.color = 2;
                    item.BackColor = Color.DarkGray;
                    //grouping
                    // checkNeighbours(dynamicButton, true, listBlacks);

                    checkNeighbours(item, false, listBlacks, previousListBlacks);
                    moves.Add((byte)item.id);
                    item.Enabled = false;


                }
            }
            //MessageBox.Show(value.ToString());
            //count steps ??
            countSteps = countSteps + 2;
            stepsLeft = stepsLeft - 2;
            stepsLeftLabel.Text = stepsLeft.ToString();
            //Calculate result
            Score(listWhites, listBlacks, out resultWhites, out resultBlacks);
            whiteScore.Text = resultWhites.ToString();
            blackScore.Text = resultBlacks.ToString();
            
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            int a = moves.Count - 1;
            if (a >= 0)
            {
                foreach (Field item in fieldList)
                {
                    if (item.id == moves[a])
                    {
                        if (item.color == 1) { removeLast(moves[a], listWhites, previousListWhites); }
                        else if (item.color == 2) { removeLast(moves[a], listBlacks, previousListBlacks); }
                        item.BackColor = SystemColors.ControlLight;
                        item.color = 0;
                        item.Enabled = true;
                    }
                }
                moves.RemoveAt(moves.Count - 1);
                player = !player;
                countSteps--;
                stepsLeft++;
                //Calculate result
                Score(listWhites, listBlacks, out resultWhites, out resultBlacks);
                whiteScore.Text = resultWhites.ToString();
                blackScore.Text = resultBlacks.ToString();
            }
        }

        private void save()
        {
            using (StreamWriter sr = new StreamWriter("important.txt"))
            {
                sr.WriteLine("save test");
                sr.Close();

            }

            using (StreamReader sr = new StreamReader("important.txt"))
            {
                String s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {

        }
    }
}

