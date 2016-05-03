using System;

namespace Othello
{
    public sealed class OthelloAIConfig
    {
        public int depth { get; set; }
        public float alpha { get; set; }
        public float beta { get;set; }
        public string turnrange { get; set; }
        public int difficulty { get; set; }

        public bool IsInRange(int Turn, GameDifficultyMode difficulty)
        {
            if (this.difficulty != (int)difficulty)
                return false;

            //Regex pattern = new Regex("(\\[|\\()[0-9]+:[0-9]+(\\]|\\])");
            

            //(0:30), (40:43]
            string[] ranges = turnrange.Split(':');

            if (ranges[0].Contains("("))
            {
                if (Turn <= int.Parse(ranges[0].Remove(0, 1)))
                    return false;
            }
            else if (ranges[0].Contains("["))
            {
                if (Turn < int.Parse(ranges[0].Remove(0, 1)))
                    return false;
            }
            else
                throw new Exception(string.Format("invalid config parameter in {0} in row: {1}\t{2}\t{3}\t{4}\t{5}",ranges[0],depth,alpha,beta,turnrange, difficulty));

            if(ranges[1].Contains(")"))
            {
                if (Turn >= int.Parse(ranges[1].TrimEnd(')')))
                    return false;
            }
            else if(ranges[1].Contains("]"))
            {
                if (Turn > int.Parse(ranges[1].TrimEnd(']')))
                    return false;
            }
            else
                throw new Exception(string.Format("invalid config parameter in {0} in row: {1}\t{2}\t{3}\t{4}\t{5}", ranges[1], depth, alpha, beta, turnrange, difficulty));

            return true;
        }
    }
}
