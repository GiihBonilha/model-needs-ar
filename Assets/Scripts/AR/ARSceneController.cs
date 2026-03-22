using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ARSceneController : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private QuestionManager questionManager;
    [SerializeField] private GameObject startMissionButton;

    private List<MissionPhase> missionPhases;
    private int currentPhaseIndex = 0;

    private void Start()
    {
        BuildMission();
    }

    public void StartMission()
    {
        startMissionButton.SetActive(false);
        currentPhaseIndex = 0;
        RunCurrentPhase();
    }

    private void RunCurrentPhase()
    {
        if (currentPhaseIndex >= missionPhases.Count)
        {
            EndMission();
            return;
        }

        MissionPhase phase = missionPhases[currentPhaseIndex];

        if (phase.dialogues != null && phase.dialogues.Count > 0)
        {
            dialogueManager.StartDialogue(phase.dialogues, () =>
            {
                if (phase.question != null)
                    questionManager.ShowQuestion(phase.question, OnAnswerSelected);
                else
                    AdvancePhase();
            });
        }
        else if (phase.question != null)
        {
            questionManager.ShowQuestion(phase.question, OnAnswerSelected);
        }
        else
        {
            AdvancePhase();
        }
    }

    private void OnAnswerSelected(AnswerOption answer)
    {
        bool isCorrect = (answer.result == AnswerResult.Correct);
        dialogueManager.PlayBotReaction(isCorrect);
        dialogueManager.ShowFeedback(answer.consequenceText, answer.explanationText, AdvancePhase);
    }

    private void AdvancePhase()
    {
        currentPhaseIndex++;
        RunCurrentPhase();
    }

    private void EndMission()
    {
        ScoreManager.Instance.SaveScore();
        SceneManager.LoadScene("ResultScene");
    }

    private void BuildMission()
    {
        missionPhases = new List<MissionPhase>();

        // INTRODUÇÃO DO BOT
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Bot, text = "Nesta missão você irá definir requisitos relacionados ao modelo de IA do sistema. A diretriz Model Needs trata das decisões sobre como o modelo será treinado, atualizado e como ele utilizará dados e feedback dos usuários." }
            }
        });

        // INTRODUÇÃO DA CLIENTE
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Cliente, text = "O sistema que queremos desenvolver foi pensado para ajudar pessoas a planejarem suas refeições de forma mais saudável, econômica e sustentável, considerando a realidade de cada uma no dia a dia." },
                new DialogueLine { character = CharacterType.Cliente, text = "Queremos que o usuário receba recomendações que façam sentido para o seu contexto, ajudando a montar refeições mais adequadas, aproveitar melhor os ingredientes disponíveis e manter uma rotina alimentar mais equilibrada." }
            }
        });

        // INTRODUÇÃO DO GERENTE
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Gerente, text = "Certo, agora temos uma compreensão mais clara do sistema. Precisamos definir com cuidado como a inteligência artificial vai operar por trás dessas recomendações. Vamos analisar como esse modelo deve ser estruturado." }
            }
        });

        // M1 — TREINAMENTO DO MODELO
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Gerente, text = "Nosso sistema recomenda refeições com base nos ingredientes disponíveis, preferências do usuário e orçamento. Precisamos decidir se o modelo de IA será treinado apenas uma vez ou se ele continuará aprendendo com o tempo." }
            },
            question = new Question
            {
                questionId = "M1",
                questionText = "O treinamento do modelo será:",
                correctAnswerIndex = 1,
                answers = new List<AnswerOption>
                {
                    new AnswerOption
                    {
                        text = "A) Estático — treinado apenas uma vez",
                        result = AnswerResult.Wrong,
                        consequenceText = "Com um modelo estático, o sistema continua recomendando cogumelos para um usuário que os rejeita repetidamente — sem nunca aprender com isso.",
                        explanationText = "Um modelo estático não se adapta ao comportamento dos usuários. Em sistemas personalizados, isso reduz a qualidade das recomendações ao longo do tempo."
                    },
                    new AnswerOption
                    {
                        text = "B) Dinâmico — atualizado continuamente",
                        result = AnswerResult.Correct,
                        consequenceText = "O sistema aprende que o usuário rejeita cebola e passa a reduzir sugestões de receitas com esse ingrediente.",
                        explanationText = "Um modelo dinâmico permite que o sistema aprenda continuamente com novos dados e feedbacks, aumentando a personalização das recomendações."
                    }
                }
            }
        });

        // M2 — TIPO DE APRENDIZADO
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Gerente, text = "Precisamos escolher como o modelo irá aprender a recomendar receitas." }
            },
            question = new Question
            {
                questionId = "M2",
                questionText = "Qual abordagem de aprendizado é mais adequada?",
                correctAnswerIndex = 0,
                answers = new List<AnswerOption>
                {
                    new AnswerOption
                    {
                        text = "A) Aprendizado supervisionado",
                        result = AnswerResult.Correct,
                        consequenceText = "O sistema usa dados históricos de receitas e preferências para aprender quais sugestões funcionam melhor.",
                        explanationText = "O aprendizado supervisionado usa dados históricos rotulados — como escolhas e rejeições anteriores dos usuários — sendo o mais adequado para sistemas de recomendação personalizada."
                    },
                    new AnswerOption
                    {
                        text = "B) Aprendizado não supervisionado",
                        result = AnswerResult.Wrong,
                        consequenceText = "O sistema pode agrupar ingredientes, mas terá dificuldade em prever preferências individuais.",
                        explanationText = "O aprendizado não supervisionado identifica padrões, mas sem exemplos claros de decisões corretas, a personalização fica limitada."
                    },
                    new AnswerOption
                    {
                        text = "C) Aprendizado por reforço",
                        result = AnswerResult.Warning,
                        consequenceText = "O sistema pode aprender com interações, mas o treinamento inicial será muito lento.",
                        explanationText = "O aprendizado por reforço aprende por tentativa e erro, exigindo muitas interações antes de produzir boas recomendações — frustrando usuários novos."
                    }
                }
            }
        });

        // M3 — USO DE FEEDBACK
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Cliente, text = "Às vezes eu não gosto de certos ingredientes, e gostaria que o sistema aprendesse isso com o tempo." }
            },
            question = new Question
            {
                questionId = "M3",
                questionText = "Como o sistema deve utilizar o feedback do usuário?",
                correctAnswerIndex = 1,
                answers = new List<AnswerOption>
                {
                    new AnswerOption
                    {
                        text = "A) Ignorar feedback do usuário",
                        result = AnswerResult.Wrong,
                        consequenceText = "O usuário continua recebendo sugestões com ingredientes que já rejeitou múltiplas vezes.",
                        explanationText = "Ignorar feedback reduz a personalização e a satisfação do usuário de forma direta."
                    },
                    new AnswerOption
                    {
                        text = "B) Usar feedback para ajustar futuras recomendações",
                        result = AnswerResult.Correct,
                        consequenceText = "O sistema aprende com ingredientes rejeitados, receitas preferidas e tempo de preparo escolhido.",
                        explanationText = "Incorporar feedback permite que o sistema refine suas previsões e se adapte às preferências individuais dos usuários."
                    },
                    new AnswerOption
                    {
                        text = "C) Apenas registrar feedback sem alterar o modelo",
                        result = AnswerResult.Warning,
                        consequenceText = "O sistema coleta informações mas não muda nada com base nelas.",
                        explanationText = "Dados coletados e não utilizados pelo modelo reduzem o potencial de melhoria do sistema — coletar sem usar é desperdiçar informação valiosa."
                    }
                }
            }
        });

        // M4 — AJUSTE DO MODELO
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Gerente, text = "Se o usuário frequentemente rejeita receitas com um ingrediente específico, o sistema deve se adaptar a isso." }
            },
            question = new Question
            {
                questionId = "M4",
                questionText = "Como essa adaptação deve acontecer?",
                correctAnswerIndex = 1,
                answers = new List<AnswerOption>
                {
                    new AnswerOption
                    {
                        text = "A) O sistema continua sugerindo os mesmos pratos",
                        result = AnswerResult.Wrong,
                        consequenceText = "O sistema se torna rígido e irrelevante para o usuário com o tempo.",
                        explanationText = "Sistemas inteligentes precisam se adaptar ao comportamento do usuário para manter a qualidade das recomendações."
                    },
                    new AnswerOption
                    {
                        text = "B) O sistema reduz sugestões com ingredientes rejeitados",
                        result = AnswerResult.Correct,
                        consequenceText = "O sistema aprende ingredientes favoritos, estilo de preparo preferido e tempo médio de preparo.",
                        explanationText = "A adaptação baseada em comportamento melhora continuamente a experiência e a relevância das recomendações."
                    },
                    new AnswerOption
                    {
                        text = "C) O sistema reinicia as preferências do usuário",
                        result = AnswerResult.Warning,
                        consequenceText = "O sistema perde todo o conhecimento acumulado sobre o usuário.",
                        explanationText = "Reiniciar preferências elimina padrões importantes aprendidos pelo modelo, prejudicando usuários recorrentes."
                    }
                }
            }
        });

        // M5 — IMPACTO DE NOVAS ENTRADAS
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Gerente, text = "Se o usuário adicionar novos ingredientes na despensa, isso deve afetar as recomendações imediatamente." }
            },
            question = new Question
            {
                questionId = "M5",
                questionText = "Qual deve ser o impacto das novas entradas no sistema?",
                correctAnswerIndex = 2,
                answers = new List<AnswerOption>
                {
                    new AnswerOption
                    {
                        text = "A) Nenhum impacto nas recomendações",
                        result = AnswerResult.Wrong,
                        consequenceText = "O usuário adiciona tomate mas o sistema continua ignorando esse ingrediente em todas as sugestões.",
                        explanationText = "Ignorar novas entradas gera recomendações desconectadas do contexto real do usuário, quebrando a utilidade do sistema."
                    },
                    new AnswerOption
                    {
                        text = "B) Impacto apenas após a próxima atualização",
                        result = AnswerResult.Warning,
                        consequenceText = "O usuário adiciona um ingrediente e as recomendações só mudam horas ou dias depois.",
                        explanationText = "Atualizações tardias reduzem a precisão no curto prazo e frustram o usuário que espera reações imediatas."
                    },
                    new AnswerOption
                    {
                        text = "C) Atualização imediata das recomendações",
                        result = AnswerResult.Correct,
                        consequenceText = "O sistema sugere imediatamente receitas que utilizam os ingredientes disponíveis.",
                        explanationText = "Atualizações imediatas mantêm as recomendações alinhadas ao contexto atual do usuário, sendo o comportamento esperado de um sistema responsivo."
                    }
                }
            }
        });

        // ENCERRAMENTO
        missionPhases.Add(new MissionPhase
        {
            dialogues = new List<DialogueLine>
            {
                new DialogueLine { character = CharacterType.Bot, text = "Reunião encerrada. Os requisitos do modelo foram definidos. Veja agora seu desempenho." }
            }
        });
    }
}