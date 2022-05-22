using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BSAkinator
{
    public partial class MainForm : Form
    {
        private StreamWriter writer;
        private StreamReader reader;
        private bool isGameRunning;
        private int answeredQuestions;

        public MainForm()
        {
            isGameRunning = false;
            answeredQuestions = 0;
            InitializeComponent();
            writer = BSAkinator.GetPrologInput();
            reader = BSAkinator.GetPrologOutput();
        }

        private void ReadQuestion()
        {
            string line = reader.ReadLine();
            if (line.StartsWith("+"))
            {
                throw new GuessedException(line.Substring(1));
            }
            if (line.StartsWith("-"))
            {
                throw new NotFoundException(line.Substring(1));
            }
            string[] parts = line.Split('|');
            label1.Text = parts[0];
            int answersCount = int.Parse(parts[1]);
            string[] answers = new string[answersCount];
            for (int i = 0; i < answersCount; i++)
            {
                answers[i] = reader.ReadLine();
            }
            FillAnswers(answers);
        }

        private void FillAnswers(string[] answers)
        {
            tableLayoutPanelAnswers.Controls.Clear();
            tableLayoutPanelAnswers.RowStyles.Clear();
            tableLayoutPanelAnswers.RowCount = answers.Length;

            for (int i = 0; i < answers.Length; i++)
            {
                string answerOption = answers[i];
                tableLayoutPanelAnswers.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanelAnswers.Controls.Add(new RadioButton() { Text = answerOption, Dock = DockStyle.Top });
            }

        }

        private void AnswerQuestion(int option)
        {
            writer.WriteLine(option + ".");
            try
            {
                ReadQuestion();
            }
            catch (GuessedException e)
            {
                tableLayoutPanelAnswers.Controls.Clear();
                label1.Text = e.Message;
                FillAnswers(new string[] { (answeredQuestions == 8) ? "Да": "Да", "Нет" });
            }
            catch (NotFoundException e)
            {
                MessageBox.Show(e.Message);
                tableLayoutPanelAnswers.Controls.Clear();
                label1.Text = e.Message;
                FillAnswers(new string[] { "Да", "Нет" });
            }
        }

        private void buttonBottom_Click(object sender, EventArgs e)
        {
            if (!isGameRunning)
            {
                // Запуск игры
                answeredQuestions = 0;
                isGameRunning = true;
                writer.WriteLine("main.");
                buttonBottom.Text = "Ответить";
                ReadQuestion();
            }
            else
            {
                // Ответить на вопрос
                for(int i = 0; i < tableLayoutPanelAnswers.Controls.Count; i++)
                {
                    RadioButton rb = (RadioButton)tableLayoutPanelAnswers.Controls[i];
                    if (rb.Checked)
                    {
                        AnswerQuestion(i + 1);
                        answeredQuestions += 1;
                    }
                }
            }
        }
    }

    class GuessedException : Exception
    {
        public GuessedException(string msg) : base(msg) { }
    };

    class NotFoundException : Exception
    {
        public NotFoundException(string msg) : base(msg) { }
    };
}
