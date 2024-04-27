using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace Grades;

public class AssessmentGrader
{
    public StudentAssessmentAssignment GradeAssessment(StudentAssessmentAssignment assignment, Assessment assessment)
    {
        try
        {
            if (!assignment.isSubmitted)
            {
                throw new ArgumentException("The assignment has not been submitted.");
            }
            
            assignment.score = CalculateOverallScore(assignment, assessment);
            assignment.topicScores = CalculateTopicScores(assignment, assessment);
            assignment.isGraded = true;
            assignment.gradedDate = DateTime.UtcNow;

            // Log a specific topic score before returning
            Console.WriteLine("Topic score before returning:");
            Console.WriteLine($"Math Addition Score: {assignment.topicScores.math.addition.score}");


            return assignment;
        }
        catch (Exception ex)
        {
            // Handle the exception here or rethrow if needed
            Console.WriteLine("An error occurred: " + ex.Message);
            throw;
        }
    }

    private double CalculateOverallScore(StudentAssessmentAssignment assignment, Assessment assessment)
    {
        try
        {
            var correctAnswers = 0;
            foreach (var answer in assignment.answers)
            {
                var question = assessment.questionsWithAnswers.FirstOrDefault(q => q.id == answer.questionId);
                if (question != null)
                {
                    var correctAnswerIds = question.answerOptions
                        .Where(option => option.isCorrect)
                        .Select(option => option.answerId);
                    if (correctAnswerIds.Contains(answer.answerId))
                    {
                        correctAnswers++;
                    }
                }
            }

            return (double)correctAnswers / assessment.questionsWithAnswers.Count * 100;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred in CalculateOverallScore: " + ex.Message);
            throw; 
        }
    }

    private TopicScores CalculateTopicScores(StudentAssessmentAssignment assignment, Assessment assessment)
    {
        var topicScores = new TopicScores();

        CountQuestionsInTopic(assessment, topicScores);

        foreach (var question in assessment.questionsWithAnswers)
        {
            var correctAnswerIds = question.answerOptions
                .Where(option => option.isCorrect)
                .Select(option => option.answerId);

            var answer = assignment.answers.FirstOrDefault(a => a.questionId == question.id);

            if (answer != null && correctAnswerIds.Contains(answer.answerId))
            {
                Console.WriteLine("Exam subject/ stream: " +  assessment.stream);
                Console.WriteLine("Question topic: " + question.topic);
                AddScore(question.topic, assessment.stream, topicScores);
            }

            
        }
        
        CalculateScoresForSubjectsAndTopics(topicScores);

        Console.WriteLine("Topic score before returning to mother grader:");
        Console.WriteLine($"Math Addition Score: {topicScores.math.addition.score}");
        Console.WriteLine($"Math Multi Score: {topicScores.math.multiplication.score}");

        

        return topicScores;
    }


    private void CalculateScoresForSubjectsAndTopics(TopicScores topicScores)
    {
        foreach (var subjectProperty in typeof(TopicScores).GetProperties())
        {
            var subject = subjectProperty.GetValue(topicScores);

            foreach (var topicProperty in subject.GetType().GetProperties())
            {
                var topic = topicProperty.GetValue(subject);

                if (topic.GetType().GetProperty("total") != null)
                {
                    double total = (double)topic.GetType().GetProperty("total").GetValue(topic);
                    if (total != 0)
                    {
                        double correct = (double)topic.GetType().GetProperty("correct").GetValue(topic);
                        double score = correct / total * 100;

                        topic.GetType().GetProperty("score").SetValue(topic, score);
                    }
                }
            }
        }
    }


    private void CountQuestionsInTopic(Assessment assessment, TopicScores topicScores)
    {
        //TODO decide wether to make this modula and able to handle new subjects added or just count them when assessment made
        // a new suvject will allways need to be added to TopicScores so maybe better to just add here to
        // Count English topics
        topicScores.english.grammar_and_punctuation.total = assessment.questionsWithAnswers.Count(q => q.topic == "grammar_and_punctuation");
        topicScores.english.reading_and_comprehension.total = assessment.questionsWithAnswers.Count(q => q.topic == "reading_and_comprehension");
        topicScores.english.spelling_and_vocabulary.total = assessment.questionsWithAnswers.Count(q => q.topic == "spelling_and_vocabulary");
        topicScores.english.writing_skills.total = assessment.questionsWithAnswers.Count(q => q.topic == "writing_skills");
        topicScores.english.comprehension_and_analysis_of_literature.total = assessment.questionsWithAnswers.Count(q => q.topic == "comprehension_and_analysis_of_literature");
        topicScores.english.sentence_completion.total = assessment.questionsWithAnswers.Count(q => q.topic == "sentence_completion");
        topicScores.english.sentence_transformation.total = assessment.questionsWithAnswers.Count(q => q.topic == "sentence_transformation");
        topicScores.english.literary_terms_and_concepts.total = assessment.questionsWithAnswers.Count(q => q.topic == "literary_terms_and_concepts");
        topicScores.english.grammar_rules_and_usage.total = assessment.questionsWithAnswers.Count(q => q.topic == "grammar_rules_and_usage");

        // Count Math topics
        topicScores.math.algebra.total = assessment.questionsWithAnswers.Count(q => q.topic == "algebra");
        topicScores.math.addition.total = assessment.questionsWithAnswers.Count(q => q.topic == "addition");
        topicScores.math.subtraction.total = assessment.questionsWithAnswers.Count(q => q.topic == "subtraction");
        topicScores.math.multiplication.total = assessment.questionsWithAnswers.Count(q => q.topic == "multiplication");
        topicScores.math.division.total = assessment.questionsWithAnswers.Count(q => q.topic == "division");
        topicScores.math.fractions.total = assessment.questionsWithAnswers.Count(q => q.topic == "fractions");
        topicScores.math.percentages.total = assessment.questionsWithAnswers.Count(q => q.topic == "percentages");
        topicScores.math.decimals.total = assessment.questionsWithAnswers.Count(q => q.topic == "decimals");
        topicScores.math.ratio_and_proportion.total = assessment.questionsWithAnswers.Count(q => q.topic == "ratio_and_proportion");
        topicScores.math.number_and_place_value.total = assessment.questionsWithAnswers.Count(q => q.topic == "number_and_place_value");
        topicScores.math.measurement.total = assessment.questionsWithAnswers.Count(q => q.topic == "measurement");
        topicScores.math.statistics.total = assessment.questionsWithAnswers.Count(q => q.topic == "statistics");

        // Generate report logic here
        Console.WriteLine("Method ran successfully.");
    
    }


    public void AddScore(string topic, string assessmentStream, TopicScores scores)
    {
        try
        {
            // Retrieve the subject property ('english' or 'math') based on the assessment stream
            var subjectProperty = scores.GetType().GetProperty(assessmentStream.ToLower());
            
            if (subjectProperty != null)
            {
                // Get the value of the subject property ('EnglishScores' or 'MathScores')
                var subjectValue = subjectProperty.GetValue(scores);

                // Retrieve the topic property ('multiplication', 'addition', etc.)
                var topicProperty = subjectValue.GetType().GetProperty(topic);
                if (topicProperty != null)
                {
                    // Get the value of the topic property ('Score', 'Correct', etc.)
                    var topicValue = topicProperty.GetValue(subjectValue);

                    // Cast topicValue to the appropriate type (Score) before accessing the 'score' property
                    if (topicValue is Score scoreValue)
                    {
                        // Update the 'Score' property of the topic
                        var scoreProperty = typeof(Score).GetProperty("correct");
                        if (scoreProperty != null)
                        {
                            Console.WriteLine("scoreProperty Updating topic correct count");
                            double currentScore = (double)scoreProperty.GetValue(scoreValue);

                            Console.WriteLine($"Current count: {currentScore}");
                            // Update the score (e.g., increment by 1)
                            scoreProperty.SetValue(scoreValue, currentScore + 1);

                            Console.WriteLine($"New count set successfully: {scoreValue.score}");
                        }
                        else
                        {
                            Console.WriteLine("Score property not found.");
                            throw new Exception("Score property not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("unable to caste to score.");
                        throw new Exception("unable to caste to score.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid topic property.");
                    throw new Exception("Invalid topic property");
                }
            }
            else
            {
                Console.WriteLine("Invalid subject property.");
                throw new Exception("Invalid subject property");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred in AddScore: " + ex.Message);
            throw; 
        }
    }
}
