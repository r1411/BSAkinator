using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace BSAkinator
{
    public partial class MainForm : Form
    {
        private StreamWriter writer;
        private StreamReader reader;
        private int answeredQuestions;
        private GameState gameState;
        SoundPlayer musicSound = new SoundPlayer(Properties.Resources.background);
        SoundPlayer revealBrawlerSound = new SoundPlayer(Properties.Resources.reveal_brawler);
        SoundPlayer revealFrankSound = new SoundPlayer(Properties.Resources.reveal_frank);
        public MainForm()
        {
            gameState = GameState.Init;
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
                throw new ResultOkException(line.Substring(1));
            }
            if (line.StartsWith("-"))
            {
                throw new ResultBadException(line.Substring(1));
            }
            if (line.StartsWith("*"))
            {
                throw new NeedInputException(line.Substring(1));
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

        private void AnswerQuestion(string option)
        {
            writer.WriteLine(option + ".");
            try
            {
                ReadQuestion();
            }
            catch (ResultOkException e)
            {
                if (gameState == GameState.Asking)
                {
                    string[] parts = e.Message.Split('|');
                    bool frank = parts[0].Contains("Фрэнк");
                    musicSound.Stop();
                    (frank ? revealFrankSound : revealBrawlerSound).Play();
                    Thread.Sleep(2000);
                    new Thread(delegate() 
                    {
                        Thread.Sleep(frank ? 9400 : 2000);
                        revealBrawlerSound.Stop();
                        musicSound.PlayLooping();
                    }).Start();
                    gameState = GameState.Guessed;
                    tableLayoutPanelAnswers.Controls.Clear();
                    label1.Text = parts[0];
                    pictureBox1.Image = Properties.Resources.bs_loading;
                    pictureBox1.LoadAsync(parts[1]);
                    FillAnswers(new string[] { "Да", "Нет" });
                }
                else if (gameState == GameState.AskingToAdd)
                {
                    MessageBox.Show(e.Message, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    reader.ReadLine();
                    reader.ReadLine();
                    ResetGame();
                }
            }
            catch (ResultBadException e)
            {
                if (gameState == GameState.Asking)
                {
                    gameState = GameState.NoAnswer;
                    tableLayoutPanelAnswers.Controls.Clear();
                    label1.Text = e.Message;
                    FillAnswers(new string[] { "Да", "Нет" });
                }
                else if (gameState == GameState.AskingToAdd)
                {
                    MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    reader.ReadLine();
                    reader.ReadLine();
                    ResetGame();
                }
            }
            catch (NeedInputException e)
            {
                EnterNewCharName(e.Message);
            }
        }

        // Отправить прологу имя нового персонажа. В msg лежит строка с приглашением ко вводу, которое нужно показать пользователю. 
        private void EnterNewCharName(string msg)
        {
            Dialog dialog = new Dialog();
            dialog.ShowDialog();
            string name = dialog.Nickname;
            string link = dialog.Link;
            name = name + ".";
            link = "'" + link + "'";
            writer.WriteLine(name); 
            AnswerQuestion(link); // TODO: Сделать диалог с вводом имени нового персонажа
        }

        private void ResetGame()
        {
            answeredQuestions = 0;
            gameState = GameState.Init;
            tableLayoutPanelAnswers.Controls.Clear();
            buttonBottom.Text = "Начать";
            label1.Text = "Привет! Я угадываю персонажей игры Brawl Stars";
            pictureBox1.Image = Properties.Resources.defi;
        }

        private void buttonBottom_Click(object sender, EventArgs e)
        {
            if (gameState == GameState.Init)
            {
                // Запуск игры
                answeredQuestions = 0;
                gameState = GameState.Asking;
                writer.WriteLine("main.");
                buttonBottom.Text = "Ответить";
                ReadQuestion();
            }
            else if (gameState == GameState.Asking || gameState == GameState.AskingToAdd)
            {
                // Ответить на вопрос
                for(int i = 0; i < tableLayoutPanelAnswers.Controls.Count; i++)
                {
                    RadioButton rb = (RadioButton)tableLayoutPanelAnswers.Controls[i];
                    if (rb.Checked)
                    {
                        AnswerQuestion((i + 1).ToString());
                        answeredQuestions += 1;
                    }
                }
            } 
            else if (gameState == GameState.Guessed)
            {
                RadioButton yes = (RadioButton)tableLayoutPanelAnswers.Controls[0];
                RadioButton no;
                if (tableLayoutPanelAnswers.Controls.Count > 1)
                    no = (RadioButton)tableLayoutPanelAnswers.Controls[1];
                else
                    no = null;

                if (yes.Checked)
                {
                    writer.WriteLine("1.");
                    reader.ReadLine();
                    reader.ReadLine();
                    reader.ReadLine();
                    ResetGame();
                } 
                else if (no != null)
                {
                    if (no.Checked)
                    {
                        pictureBox1.Image = Properties.Resources.defi;
                        gameState = GameState.AskingToAdd;
                        AnswerQuestion("2");
                    }
                } 
                else
                {
                    return;
                }
            }
            else if (gameState == GameState.NoAnswer)
            {
                RadioButton yes = (RadioButton)tableLayoutPanelAnswers.Controls[0];
                RadioButton no = (RadioButton)tableLayoutPanelAnswers.Controls[1];

                if (yes.Checked)
                {
                    gameState = GameState.AskingToAdd;
                    AnswerQuestion("1");
                }
                else if (no.Checked)
                {
                    writer.WriteLine("2.");
                    reader.ReadLine();
                    reader.ReadLine();
                    reader.ReadLine();
                    ResetGame();
                }
                else
                {
                    return;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            musicSound.PlayLooping();
        }
    }
    class ResultOkException : Exception
    {
        public ResultOkException(string msg) : base(msg) { }
    };

    class ResultBadException : Exception
    {
        public ResultBadException(string msg) : base(msg) { }
    };

    class NeedInputException : Exception
    {
        public NeedInputException(string msg) : base(msg) { }
    };
}
