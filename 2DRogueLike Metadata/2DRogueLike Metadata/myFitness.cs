using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using System.Windows.Forms;

namespace _2DRogueLike_Metadata
{
    class myFitness : GeneticSharp.Domain.Fitnesses.IFitness
    {
        double answer;
        public double Evaluate(IChromosome chromosome)
        {
            //throw new NotImplementedException();
            using (var prompt = new _2DRogueLike_Metadata.Painter(chromosome.GetGenes()))
            {
                var result = prompt.ShowDialog();
                if (result == DialogResult.OK)
                {
                    double answer = prompt.answer;            //values preserved after close

                    this.answer = answer;
                }
            }

            return answer;
        }
    }
}
