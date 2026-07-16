############################################################
# Analysis Script - RE4AI-AR Experiment
# Author: Anonymous
# Description: Statistical analysis of accuracy data
############################################################

library(ggplot2)

# Controle (Papel)
controle <- c(
  80,100,80,80,40,60,40,80,80,60,80,80,
  80,60,80,60,60,80,80,40,80,80,80,80
)

# Experimental (App)
experimental <- c(
  80,100,80,100,80,100,80,80,60,80,100,80,
  80,100,80,100,80,100,80,80,60,80,80,100
)

grupo <- c(rep("Controle", length(controle)),
           rep("Experimental", length(experimental)))

acuracia <- c(controle, experimental)

dados <- data.frame(
  Grupo = grupo,
  Acuracia = acuracia
)

# Estatísticas descritivas
print(summary(controle))
print(summary(experimental))

cat("\nMédia Controle:", mean(controle))
cat("\nMédia Experimental:", mean(experimental))

cat("\nDesvio Padrão Controle:", sd(controle))
cat("\nDesvio Padrão Experimental:", sd(experimental))

# Teste de normalidade
print(shapiro.test(controle))
print(shapiro.test(experimental))

# Mann-Whitney
teste_mw <- wilcox.test(controle, experimental)
print(teste_mw)

# Effect size (r)
n1 <- length(controle)
n2 <- length(experimental)

U <- teste_mw$statistic
mu_U <- n1 * n2 / 2
sigma_U <- sqrt(n1 * n2 * (n1 + n2 + 1) / 12)

Z <- (U - mu_U) / sigma_U
r <- abs(Z) / sqrt(n1 + n2)

cat("\nEffect Size (r):", r)

# Boxplot
ggplot(dados, aes(x = Grupo, y = Acuracia)) +
  geom_boxplot() +
  labs(title = "Distribuição da Acurácia por Grupo",
       x = "Grupo",
       y = "Acurácia (%)") +
  theme_minimal()

ggsave("boxplot_acuracia.png")
