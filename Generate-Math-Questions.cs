using Model;
using System.Collections.Generic;

namespace seed;

public partial class generateQuestions {
    public List<Question> GenerateMathQuestions()
    {
        
        var mathQuestions = new List<Question>
        {
            // Number and Place Value Questions
            new Question
            {
                questionText = "What is the value of the digit in the thousands place in the number 54,321?",
                topic = "number_and_place_value",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 0, answerText = "4", isCorrect = true },
                    new AnswerOption { answerId = 1, answerText = "5", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Write the number 'seven thousand, three hundred twenty-four' in standard form.",
                topic = "number_and_place_value",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 2, answerText = "7,324", isCorrect = true },
                    new AnswerOption { answerId = 3, answerText = "7,342", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Round 67,892 to the nearest thousand.",
                topic = "number_and_place_value",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 4, answerText = "68,000", isCorrect = true },
                    new AnswerOption { answerId = 5, answerText = "67,000", isCorrect = false }
                }
            },
            // Addition Questions
            new Question
            {
                questionText = "Calculate: 345 + 287",
                topic = "addition",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 6, answerText = "632", isCorrect = true },
                    new AnswerOption { answerId = 7, answerText = "628", isCorrect = false }
                }
            },

            // Subtraction Questions
            new Question
            {
                questionText = "Subtract 462 from 987.",
                topic = "subtraction",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 8, answerText = "525", isCorrect = true },
                    new AnswerOption { answerId = 9, answerText = "549", isCorrect = false }
                }
            },

            // Multiplication Questions
            new Question
            {
                questionText = "What is 18 x 7?",
                topic = "multiplication",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 10, answerText = "126", isCorrect = true },
                    new AnswerOption { answerId = 11, answerText = "125", isCorrect = false }
                }
            },

            // Division Questions
            new Question
            {
                questionText = "Divide 144 by 12.",
                topic = "division",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 12, answerText = "12", isCorrect = true },
                    new AnswerOption { answerId = 13, answerText = "11", isCorrect = false }
                }
            },

            // Fractions Questions
            new Question
            {
                questionText = "What is 3/4 as a decimal?",
                topic = "fractions",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 14, answerText = "0.75", isCorrect = true },
                    new AnswerOption { answerId = 15, answerText = "0.25", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Add the fractions: 1/4 + 3/8",
                topic = "fractions",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 16, answerText = "5/8", isCorrect = true },
                    new AnswerOption { answerId = 17, answerText = "7/8", isCorrect = false }
                }
            },

            // Percentages Questions
            new Question
            {
                questionText = "Calculate: 25% of 80",
                topic = "percentages",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 18, answerText = "20", isCorrect = true },
                    new AnswerOption { answerId = 19, answerText = "25", isCorrect = false }
                }
            },

            // Decimals Questions
            new Question
            {
                questionText = "What is 0.75 as a fraction?",
                topic = "decimals",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 20, answerText = "3/4", isCorrect = true },
                    new AnswerOption { answerId = 21, answerText = "4/3", isCorrect = false }
                }
            },

            // ratio_and_proportion Questions
            new Question
            {
                questionText = "If the ratio of boys to girls in a class is 3:5, and there are 24 students in total, how many boys are in the class?",
                topic = "ratio_and_proportion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 22, answerText = "9", isCorrect = true },
                    new AnswerOption { answerId = 23, answerText = "15", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "If 3 apples cost $2, how much would 9 apples cost?",
                topic = "ratio_and_proportion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 24, answerText = "$6", isCorrect = true },
                    new AnswerOption { answerId = 25, answerText = "$4", isCorrect = false }
                }
            },

            // Algebra Questions
            new Question
            {
                questionText = "Solve for x: 3x + 5 = 14",
                topic = "algebra",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 26, answerText = "3", isCorrect = true },
                    new AnswerOption { answerId = 27, answerText = "5", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Simplify the expression: 2x + 3x",
                topic = "algebra",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 28, answerText = "5x", isCorrect = true },
                    new AnswerOption { answerId = 29, answerText = "6x", isCorrect = false }
                }
            },

            // Measurement Questions
            new Question
            {
                questionText = "Convert 3.5 meters to centimeters.",
                topic = "measurement",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 30, answerText = "350", isCorrect = true },
                    new AnswerOption { answerId = 31, answerText = "35", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "A rectangular room has a length of 8 meters and a width of 5 meters. What is the area of the room?",
                topic = "measurement",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 32, answerText = "40 square meters", isCorrect = true },
                    new AnswerOption { answerId = 33, answerText = "13 square meters", isCorrect = false }
                }
            },

            // statistics Questions
            new Question
            {
                questionText = "The ages of students in a class are: 12, 13, 14, 12, 15, 13, 14, 12. What is the mode of the data set?",
                topic = "statistics",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 34, answerText = "12", isCorrect = true },
                    new AnswerOption { answerId = 35, answerText = "13", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "The heights (in cm) of a group of people are: 165, 172, 160, 168, 175, 162. What is the mean height?",
                topic = "statistics",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 36, answerText = "167", isCorrect = true },
                    new AnswerOption { answerId = 37, answerText = "170", isCorrect = false }
                }
            }
        };

        return mathQuestions;
    }
}