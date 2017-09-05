using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Domain.Randomizations;

namespace _2DRogueLike_Metadata
{
    class myChromosome : GeneticSharp.Domain.Chromosomes.IBinaryChromosome
    {

        private double[] m_minValue;
        private double[] m_maxValue;
        private int[] m_totalBits;
        private int[] m_fractionBits;
        private string m_originalValueStringRepresentation;

        private Gene[] m_genes;
        private int m_length;

        public myChromosome(double minValue, double maxValue, int fractionBits) 
			: this(new double[] { minValue }, new double[] { maxValue }, new int[] { 32 }, new int[] { fractionBits })
		{
        }

        public myChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] fractionBits)
            //: base(totalBits.Sum())
        {

            m_minValue = minValue;
            m_maxValue = maxValue;
            m_totalBits = totalBits;
            m_fractionBits = fractionBits;

            var originalValues = new double[minValue.Length];
            var rnd = RandomizationProvider.Current;

            for (int i = 0; i < originalValues.Length; i++)
            {
                originalValues[i] = rnd.GetDouble(minValue[i], maxValue[i]);
            }

            m_originalValueStringRepresentation = String.Join(
                "",
                BinaryStringRepresentation.ToRepresentation(
                originalValues,
                totalBits,
                    fractionBits));

            CreateGenes();
        }
        public double? Fitness { get; set; }

        public int Length
        {
            get
            {
                //throw new NotImplementedException();
                return m_length;
            }
        }

        public virtual IChromosome Clone()
        {
            var clone = CreateNew();
            clone.ReplaceGenes(0, GetGenes());
            clone.Fitness = Fitness;

            return clone;
        }

        public int CompareTo(IChromosome other)
        {
            if (other == null)
            {
                return -1;
            }

            var otherFitness = other.Fitness;

            if (Fitness == otherFitness)
            {
                return 0;
            }

            // TODO: chromosomes with same fitnesss are really equals?
            return Fitness > otherFitness ? 1 : -1;
        }

        public IChromosome CreateNew()
        {
            //throw new NotImplementedException();
            return new FloatingPointChromosome(m_minValue, m_maxValue, m_totalBits, m_fractionBits);
        }

        public void FlipGene(int index)
        {
            //throw new NotImplementedException();
            var value = (int)GetGene(index).Value;

            ReplaceGene(index, new Gene(value == 0 ? 1 : 0));
        }

        public Gene GenerateGene(int geneIndex)
        {
            //throw new NotImplementedException();
            return new Gene(Convert.ToInt32(m_originalValueStringRepresentation[geneIndex].ToString()));

        }

        public Gene GetGene(int index)
        {
            return m_genes[index];
        }

        public Gene[] GetGenes()
        {
            return m_genes;
        }

        public void ReplaceGene(int index, Gene gene)
        {
            if (index < 0 || index >= m_length)
            {
                throw new ArgumentOutOfRangeException("index", "There is no Gene on index {0} to be replaced.".With(index));
            }

            m_genes[index] = gene;
            Fitness = null;
        }

        public void ReplaceGenes(int startIndex, Gene[] genes)
        {
            ExceptionHelper.ThrowIfNull("genes", genes);

            if (genes.Length > 0)
            {
                if (startIndex < 0 || startIndex >= m_length)
                {
                    throw new ArgumentOutOfRangeException("index", "There is no Gene on index {0} to be replaced.".With(startIndex));
                }

                var genesToBeReplacedLength = genes.Length;

                var availableSpaceLength = m_length - startIndex;

                if (genesToBeReplacedLength > availableSpaceLength)
                {
                    throw new ArgumentException(
                        "genes", "The number of genes to be replaced is greater than available space, there is {0} genes between the index {1} and the end of chromosome, but there is {2} genes to be replaced.".With(availableSpaceLength, startIndex, genesToBeReplacedLength));
                }

                Array.Copy(genes, 0, m_genes, startIndex, genes.Length);

                Fitness = null;
            }
        }
        
        public void Resize(int newLength)
        {
            ValidateLength(newLength);

            Array.Resize(ref m_genes, newLength);
            m_length = newLength;
        }

        public double ToFloatingPoint()
        {
            return EnsureMinMax(
                BinaryStringRepresentation.ToDouble(ToString()),
                0);

        }

        /// <summary>
        /// Converts the chromosome to the floating points representation.
        /// </summary>
        /// <returns>The floating points.</returns>
        public double[] ToFloatingPoints()
        {
            return BinaryStringRepresentation
                .ToDouble(ToString(), m_totalBits, m_fractionBits)
                .Select(EnsureMinMax)
                .ToArray();
        }

        private double EnsureMinMax(double value, int index)
        {
            if (value < m_minValue[index])
            {
                return m_minValue[index];
            }
            else if (value > m_maxValue[index])
            {
                return m_maxValue[index];
            }

            return value;
        }

        public override string ToString()
        {
            return String.Join("", GetGenes().Select(g => g.Value.ToString()).ToArray());
        }

        public override bool Equals(object obj)
        {
            var other = obj as IChromosome;

            if (other == null)
            {
                return false;
            }

            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return Fitness.GetHashCode();
        }

        protected virtual void CreateGene(int index)
        {
            ReplaceGene(index, GenerateGene(index));
        }

        protected virtual void CreateGenes()
        {
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        private static void ValidateLength(int length)
        {
            if (length < 2)
            {
                throw new ArgumentException("The minimum length for a chromosome is 2 genes.");
            }
        }


    }
}
