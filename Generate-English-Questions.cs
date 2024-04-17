using Model;
using System.Collections.Generic;

namespace seed;

public partial class generateQuestions {
    public List<Question> GenerateEnglishQuestions()
    {
        // Generate a list of realistic English questions with topics and correct answers
        var englishQuestions = new List<Question>
        {
            // Grammar and Punctuation Questions
            new Question
            {
                questionText = "Select the correct sentence: 'She is going to the store.'",
                topic = "grammar_and_punctuation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 0, answerText = "She is going to the store.", isCorrect = true },
                    new AnswerOption { answerId = 1, answerText = "She is going to the store?", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the plural form of 'child'?",
                topic = "grammar_and_punctuation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 2, answerText = "Children", isCorrect = true },
                    new AnswerOption { answerId = 3, answerText = "Childs", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Choose the correct word to complete the sentence: 'She ____ a book yesterday.'",
                topic = "grammar_and_punctuation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 4, answerText = "read", isCorrect = true },
                    new AnswerOption { answerId = 5, answerText = "red", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Which of the following sentences contains correct punctuation?",
                topic = "grammar_and_punctuation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 6, answerText = "I love to read books.", isCorrect = true },
                    new AnswerOption { answerId = 7, answerText = "I love to read books", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the main difference between 'your' and 'you're'?",
                topic = "grammar_and_punctuation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 8, answerText = "'Your' is possessive, 'you're' is a contraction of 'you are'.", isCorrect = true },
                    new AnswerOption { answerId = 9, answerText = "They mean the same thing.", isCorrect = false }
                }
            },

            // reading_and_comprehension Questions
            new Question
            {
                questionText = "Read the following sentence: 'The cat chased the mouse.' What did the cat do?",
                topic = "reading_and_comprehension",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 10, answerText = "Chased the mouse", isCorrect = true },
                    new AnswerOption { answerId = 11, answerText = "Ate the mouse", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the opposite of 'happy'?",
                topic = "reading_and_comprehension",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 12, answerText = "Sad", isCorrect = true },
                    new AnswerOption { answerId = 13, answerText = "Joyful", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the main idea of the passage?",
                topic = "reading_and_comprehension",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 14, answerText = "The importance of exercise", isCorrect = true },
                    new AnswerOption { answerId = 15, answerText = "The history of bicycles", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the author's tone in the passage?",
                topic = "reading_and_comprehension",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 16, answerText = "Informative", isCorrect = true },
                    new AnswerOption { answerId = 17, answerText = "Humorous", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the main idea of the story?",
                topic = "reading_and_comprehension",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 18, answerText = "The adventures of a young girl", isCorrect = true },
                    new AnswerOption { answerId = 19, answerText = "The history of a famous scientist", isCorrect = false }
                }
            },
            
            // spelling_and_vocabulary Questions
            new Question
            {
                questionText = "Spell the word 'necessary'.",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 20, answerText = "Necessary", isCorrect = true },
                    new AnswerOption { answerId = 21, answerText = "Necesary", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Which word means 'to make up for something'?",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 22, answerText = "Compensate", isCorrect = true },
                    new AnswerOption { answerId = 23, answerText = "Exaggerate", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Use 'ecstatic' in a sentence.",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 24, answerText = "She was ecstatic when she received the award.", isCorrect = true },
                    new AnswerOption { answerId = 25, answerText = "She was tired after the long day.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the synonym of 'generous'?",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 27, answerText = "Benevolent", isCorrect = true },
                    new AnswerOption { answerId = 28, answerText = "Greedy", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What does the word 'eloquent' mean?",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 29, answerText = "Expressive and persuasive in speech", isCorrect = true },
                    new AnswerOption { answerId = 30, answerText = "Unable to speak clearly", isCorrect = false }
                }
            }, 

            // sentence_completion Questions
            new Question
            {
                questionText = "Complete the sentence: 'The cat ____ on the windowsill.'" ,
                topic = "sentence_completion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 51, answerText = "slept", isCorrect = true },
                    new AnswerOption { answerId = 52, answerText = "barked", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Finish the sentence: 'The sun was shining, and the sky was ____.'",
                topic = "sentence_completion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 53, answerText = "clear", isCorrect = true },
                    new AnswerOption { answerId = 54, answerText = "dark", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Complete the sentence: 'She played the piano with ____ skill.'",
                topic = "sentence_completion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 55, answerText = "exceptional", isCorrect = true },
                    new AnswerOption { answerId = 56, answerText = "poor", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Finish the sentence: 'The flowers in the garden were ____ in color.'",
                topic = "sentence_completion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 57, answerText = "vibrant", isCorrect = true },
                    new AnswerOption { answerId = 58, answerText = "dull", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Complete the sentence: 'The teacher praised her for her ____ effort.'",
                topic = "sentence_completion",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 59, answerText = "diligent", isCorrect = true },
                    new AnswerOption { answerId = 60, answerText = "careless", isCorrect = false }
                }
            },

                    // sentence_transformation Questions
            new Question
            {
                questionText = "Change the following sentence from active voice to passive voice: 'The cat chased the mouse.'",
                topic = "sentence_transformation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 61, answerText = "The mouse was chased by the cat.", isCorrect = true },
                    new AnswerOption { answerId = 62, answerText = "The cat was chasing the mouse.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Rewrite the sentence using a different word: 'She was very happy.'",
                topic = "sentence_transformation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 63, answerText = "She was extremely joyful.", isCorrect = true },
                    new AnswerOption { answerId = 64, answerText = "She was very sad.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Change the sentence to a question: 'They are coming to the party.'",
                topic = "sentence_transformation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 65, answerText = "Are they coming to the party?", isCorrect = true },
                    new AnswerOption { answerId = 66, answerText = "They are not coming to the party.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Rewrite the sentence using a synonym: 'The garden is beautiful.'",
                topic = "sentence_transformation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 67, answerText = "The garden is lovely.", isCorrect = true },
                    new AnswerOption { answerId = 68, answerText = "The garden is ugly.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Change the sentence to the past tense: 'She writes a letter.'",
                topic = "sentence_transformation",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 69, answerText = "She wrote a letter.", isCorrect = true },
                    new AnswerOption { answerId = 70, answerText = "She will write a letter.", isCorrect = false }
                }
            },

            // literary_terms_and_concepts Questions
            new Question
            {
                questionText = "What literary device is used in the following sentence: 'The stars danced in the night sky.'",
                topic = "literary_terms_and_concepts",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 71, answerText = "Personification", isCorrect = true },
                    new AnswerOption { answerId = 72, answerText = "Simile", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Identify the theme of the story: 'A story about the triumph of good over evil.'",
                topic = "literary_terms_and_concepts",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 73, answerText = "Conflict and Resolution", isCorrect = true },
                    new AnswerOption { answerId = 74, answerText = "Friendship", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What literary term refers to the repetition of consonant sounds at the beginning of words in a sentence?",
                topic = "literary_terms_and_concepts",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 75, answerText = "Alliteration", isCorrect = true },
                    new AnswerOption { answerId = 76, answerText = "Metaphor", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "In literature, what does 'foreshadowing' mean?",
                topic = "literary_terms_and_concepts",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 77, answerText = "Giving hints or clues about future events", isCorrect = true },
                    new AnswerOption { answerId = 78, answerText = "Describing past events", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the term for a figure of speech that compares two unlike things using 'like' or 'as'?",
                topic = "literary_terms_and_concepts",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 79, answerText = "Simile", isCorrect = true },
                    new AnswerOption { answerId = 80, answerText = "Metaphor", isCorrect = false }
                }
            },

                    // spelling_and_vocabulary Questions
            new Question
            {
                questionText = "Spell the word 'necessary'.",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 81, answerText = "Necessary", isCorrect = true },
                    new AnswerOption { answerId = 82, answerText = "Necesary", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Which word means 'to make up for something'?",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 83, answerText = "Compensate", isCorrect = true },
                    new AnswerOption { answerId = 84, answerText = "Exaggerate", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Use 'ecstatic' in a sentence.",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 85, answerText = "She was ecstatic when she received the award.", isCorrect = true },
                    new AnswerOption { answerId = 86, answerText = "She was tired after the long day.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the synonym of 'generous'?",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 87, answerText = "Benevolent", isCorrect = true },
                    new AnswerOption { answerId = 88, answerText = "Greedy", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What does the word 'eloquent' mean?",
                topic = "spelling_and_vocabulary",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 89, answerText = "Expressive and persuasive in speech", isCorrect = true },
                    new AnswerOption { answerId = 90, answerText = "Unable to speak clearly", isCorrect = false }
                }
            },

            // grammar_rules_and_usage Questions
            new Question
            {
                questionText = "Choose the correct verb form to complete the sentence: 'She ____ to the store yesterday.'",
                topic = "grammar_rules_and_usage",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 91, answerText = "went", isCorrect = true },
                    new AnswerOption { answerId = 92, answerText = "goed", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Identify the subject in the following sentence: 'The cat chased the mouse.'",
                topic = "grammar_rules_and_usage",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 93, answerText = "The cat", isCorrect = true },
                    new AnswerOption { answerId = 94, answerText = "chased the mouse", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Which sentence is grammatically correct: 'I have went to the park' or 'I have gone to the park'?",
                topic = "grammar_rules_and_usage",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 95, answerText = "I have gone to the park", isCorrect = true },
                    new AnswerOption { answerId = 96, answerText = "I have went to the park", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "Correct the following sentence: 'He dont like vegetables.'",
                topic = "grammar_rules_and_usage",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 97, answerText = "He doesn't like vegetables.", isCorrect = true },
                    new AnswerOption { answerId = 98, answerText = "He don't like vegetables.", isCorrect = false }
                }
            },
            new Question
            {
                questionText = "What is the correct punctuation for the following sentence: 'I love reading books'",
                topic = "grammar_rules_and_usage",
                answerOptions = new List<AnswerOption>
                {
                    new AnswerOption { answerId = 99, answerText = "I love reading books.", isCorrect = true },
                    new AnswerOption { answerId = 100, answerText = "I love reading books", isCorrect = false }
                }
            }
        
        };

        return englishQuestions;
    }
}
