using TalentDesk.Models;

namespace TalentDesk.Repositories
{
    // Currículos de exemplo gravados no banco na primeira execução
    public static class DadosIniciais
    {
        private static DateTime DiasAtras(int dias) => DateTime.Now.AddDays(-dias).AddHours(-2);

        public static List<Curriculo> Curriculos() => new()
        {
            new Curriculo
            {
                Nome = "Gustavo Calderon Moreira",
                Cargo = "Analista de RH Pleno",
                Area = "Tecnologia da Informação",
                Formacao = "Desenvolvimento de Software",
                FormacaoDetalhada = "Ciência da Computação – Bacharelado",
                Instituicao = "Universidade de São Paulo (USP)",
                ConclusaoFormacao = "Dez/2018",
                Idade = 28,
                AnosExperiencia = 5,
                FotoUrl = "https://i.pravatar.cc/150?img=32",
                Email = "gustavo.calderon@email.com",
                Telefone = "(11) 98765-4321",
                Cidade = "São Paulo, SP",
                Linkedin = "linkedin.com/in/gustavocalderon",
                Resumo = "Profissional de Recursos Humanos com experiência em recrutamento e seleção, desenvolvimento organizacional e gestão de pessoas. Apaixonado por conectar talentos e impulsionar resultados através de pessoas.",
                Status = StatusCurriculo.EmAnalise,
                DataCadastro = DiasAtras(2),
                Experiencias = new()
                {
                    new() { Cargo = "Analista de RH Pleno", Empresa = "Empresa X", Periodo = "Jan/2022 – Atual", Descricao = "Atuação em recrutamento e seleção, treinamento e desenvolvimento, onboarding e gestão de benefícios." },
                    new() { Cargo = "Analista de RH Júnior", Empresa = "Empresa Y", Periodo = "Jun/2019 – Dez/2021", Descricao = "Apoio nos processos de R&S, integração de colaboradores e ações de endomarketing." }
                },
                Habilidades = new() { "Recrutamento e Seleção", "Treinamento e Desenvolvimento", "Comunicação", "Liderança", "Pacote Office", "Excel" }
            },
            new Curriculo
            {
                Nome = "Felipe Castro",
                Cargo = "Desenvolvedor de Software",
                Area = "Desenvolvimento de Software",
                Formacao = "Ciência da Computação",
                FormacaoDetalhada = "Ciência da Computação – Bacharelado",
                Instituicao = "Universidade Federal do Rio de Janeiro (UFRJ)",
                ConclusaoFormacao = "Dez/2020",
                Idade = 25,
                AnosExperiencia = 4,
                FotoUrl = "https://i.pravatar.cc/150?img=12",
                Email = "felipe.castro@email.com",
                Telefone = "(21) 99876-1122",
                Cidade = "Rio de Janeiro, RJ",
                Linkedin = "linkedin.com/in/felipecastro",
                Resumo = "Desenvolvedor com foco em aplicações web, arquitetura de software e entrega de soluções escaláveis para negócios digitais.",
                Status = StatusCurriculo.Novo,
                DataCadastro = DiasAtras(3),
                Experiencias = new()
                {
                    new() { Cargo = "Desenvolvedor Front-end", Empresa = "Empresa A", Periodo = "Mai/2021 – Atual", Descricao = "Criação de interfaces modernas e experiências de usuário de alta performance." },
                    new() { Cargo = "Estagiário de TI", Empresa = "Empresa B", Periodo = "Jan/2019 – Abr/2021", Descricao = "Manutenção de sistemas, testes e suporte técnico em aplicações web." }
                },
                Habilidades = new() { "React", "C#", "JavaScript", "HTML", "CSS", "SQL" }
            },
            new Curriculo
            {
                Nome = "Juliana Gomes",
                Cargo = "Analista de Marketing",
                Area = "Marketing",
                Formacao = "Publicidade e Propaganda",
                FormacaoDetalhada = "Publicidade e Propaganda – Bacharelado",
                Instituicao = "PUC Minas",
                ConclusaoFormacao = "Jul/2019",
                Idade = 27,
                AnosExperiencia = 6,
                FotoUrl = "https://i.pravatar.cc/150?img=47",
                Email = "juliana.gomes@email.com",
                Telefone = "(31) 99123-4567",
                Cidade = "Belo Horizonte, MG",
                Linkedin = "linkedin.com/in/julianagomes",
                Resumo = "Especialista em campanhas digitais, branding e comunicação estratégica com foco em geração de demanda e crescimento de marcas.",
                Status = StatusCurriculo.Entrevista,
                DataCadastro = DiasAtras(5),
                Experiencias = new()
                {
                    new() { Cargo = "Coordenadora de Marketing", Empresa = "Empresa Z", Periodo = "Ago/2021 – Atual", Descricao = "Planejamento de campanhas e liderança de estratégias de comunicação digital." },
                    new() { Cargo = "Analista de Marketing", Empresa = "Empresa W", Periodo = "Jan/2019 – Jul/2021", Descricao = "Gestão de tráfego pago, social media e conteúdo institucional." }
                },
                Habilidades = new() { "Marketing Digital", "Branding", "SEO", "Google Ads", "Tráfego Pago", "Social Media" }
            },
            new Curriculo
            {
                Nome = "Rafael Silva",
                Cargo = "Cientista de Dados",
                Area = "Análise de Dados",
                Formacao = "Estatística",
                FormacaoDetalhada = "Estatística – Bacharelado",
                Instituicao = "Universidade Federal do Paraná (UFPR)",
                ConclusaoFormacao = "Dez/2019",
                Idade = 26,
                AnosExperiencia = 5,
                FotoUrl = "https://i.pravatar.cc/150?img=18",
                Email = "rafael.silva@email.com",
                Telefone = "(41) 98877-6655",
                Cidade = "Curitiba, PR",
                Linkedin = "linkedin.com/in/rafaelsilva",
                Resumo = "Analista de dados com experiência em modelagem, visualização de indicadores e apoio à tomada de decisão com dados de negócio.",
                Status = StatusCurriculo.EmAnalise,
                DataCadastro = DiasAtras(8),
                Experiencias = new()
                {
                    new() { Cargo = "Analista de Dados", Empresa = "Empresa K", Periodo = "Fev/2022 – Atual", Descricao = "Construção de dashboards, relatórios e automações para inteligência de negócio." },
                    new() { Cargo = "BI Analyst", Empresa = "Empresa M", Periodo = "Jul/2019 – Jan/2022", Descricao = "Tratamento de dados, geração de KPIs e suporte à área comercial." }
                },
                Habilidades = new() { "Power BI", "SQL", "Python", "Excel Avançado", "Dashboard", "Business Intelligence" }
            },
            new Curriculo
            {
                Nome = "Larissa Albuquerque",
                Cargo = "Designer de Produto",
                Area = "Design",
                Formacao = "Design Gráfico",
                FormacaoDetalhada = "Design Gráfico – Tecnólogo",
                Instituicao = "Universidade de Fortaleza (Unifor)",
                ConclusaoFormacao = "Dez/2020",
                Idade = 24,
                AnosExperiencia = 4,
                FotoUrl = "https://i.pravatar.cc/150?img=20",
                Email = "larissa.albuquerque@email.com",
                Telefone = "(85) 98745-1123",
                Cidade = "Fortaleza, CE",
                Linkedin = "linkedin.com/in/larissaalbuquerque",
                Resumo = "Designer visual com foco em identidade visual, UI/UX e materiais de comunicação para marcas e produtos digitais.",
                Status = StatusCurriculo.Novo,
                DataCadastro = DiasAtras(9),
                Experiencias = new()
                {
                    new() { Cargo = "Designer Pleno", Empresa = "Empresa T", Periodo = "Ago/2021 – Atual", Descricao = "Criação de sistemas visuais e desenvolvimento de interfaces responsivas." },
                    new() { Cargo = "Designer Jr.", Empresa = "Empresa P", Periodo = "Jan/2019 – Jul/2021", Descricao = "Produção de peças digitais e apoio em campanhas de comunicação." }
                },
                Habilidades = new() { "UI/UX", "Adobe Creative Suite", "Figma", "Branding", "Illustrator", "Photoshop" }
            },
            new Curriculo
            {
                Nome = "Victor Pereira",
                Cargo = "Analista Administrativo",
                Area = "Administração",
                Formacao = "Administração",
                FormacaoDetalhada = "Administração – Bacharelado",
                Instituicao = "Universidade Presbiteriana Mackenzie",
                ConclusaoFormacao = "Dez/2017",
                Idade = 29,
                AnosExperiencia = 7,
                FotoUrl = "https://i.pravatar.cc/150?img=27",
                Email = "victor.pereira@email.com",
                Telefone = "(11) 99568-3468",
                Cidade = "São Paulo, SP",
                Linkedin = "linkedin.com/in/victorpereira",
                Resumo = "Gestor administrativo com experiência em processos internos, organização operacional e suporte à liderança em ambientes dinâmicos.",
                Status = StatusCurriculo.Aprovado,
                DataCadastro = DiasAtras(20),
                Experiencias = new()
                {
                    new() { Cargo = "Analista Administrativo", Empresa = "Empresa L", Periodo = "Mar/2021 – Atual", Descricao = "Acompanhamento de rotinas administrativas, contratos e otimização de processos internos." },
                    new() { Cargo = "Assistente Administrativo", Empresa = "Empresa S", Periodo = "Jan/2018 – Fev/2021", Descricao = "Atendimento, organização documental e apoio na gestão de operações corporativas." }
                },
                Habilidades = new() { "Gestão Administrativa", "Organização", "Atendimento", "Processos", "Conciliação", "Excel" }
            },
            new Curriculo
            {
                Nome = "Amanda Bonfim",
                Cargo = "Analista de Recursos Humanos",
                Area = "Recursos Humanos",
                Formacao = "Psicologia",
                FormacaoDetalhada = "Psicologia – Bacharelado",
                Instituicao = "Universidade de São Paulo (USP)",
                ConclusaoFormacao = "Dez/2018",
                Idade = 28,
                AnosExperiencia = 5,
                FotoUrl = "https://i.pravatar.cc/150?img=45",
                Email = "amanda.bonfim@email.com",
                Telefone = "(11) 97654-3210",
                Cidade = "São Paulo, SP",
                Linkedin = "linkedin.com/in/amandabonfim",
                Resumo = "Profissional de Recursos Humanos com experiência em recrutamento e seleção, desenvolvimento organizacional e gestão de pessoas.",
                Status = StatusCurriculo.Novo,
                DataCadastro = DiasAtras(1),
                Experiencias = new()
                {
                    new() { Cargo = "Analista de RH Pleno", Empresa = "Empresa X", Periodo = "Jan/2022 – Atual", Descricao = "Recrutamento e seleção, onboarding e gestão de benefícios." },
                    new() { Cargo = "Analista de RH Júnior", Empresa = "Empresa Y", Periodo = "Jun/2019 – Dez/2021", Descricao = "Apoio nos processos de R&S e ações de endomarketing." }
                },
                Habilidades = new() { "Recrutamento e Seleção", "Desenvolvimento de Pessoas", "Comunicação", "Trabalho em Equipe", "Pacote Office" }
            },
            new Curriculo
            {
                Nome = "Bruno Almeida",
                Cargo = "Desenvolvedor Back-end",
                Area = "Desenvolvimento de Software",
                Formacao = "Sistemas de Informação",
                FormacaoDetalhada = "Sistemas de Informação – Bacharelado",
                Instituicao = "Universidade Federal de Pernambuco (UFPE)",
                ConclusaoFormacao = "Dez/2014",
                Idade = 34,
                AnosExperiencia = 11,
                FotoUrl = "https://i.pravatar.cc/150?img=52",
                Email = "bruno.almeida@email.com",
                Telefone = "(81) 99811-2233",
                Cidade = "Recife, PE",
                Linkedin = "linkedin.com/in/brunoalmeida",
                Resumo = "Desenvolvedor back-end com foco em APIs, bancos de dados e arquitetura em nuvem.",
                Status = StatusCurriculo.Entrevista,
                DataCadastro = DiasAtras(12),
                Experiencias = new()
                {
                    new() { Cargo = "Desenvolvedor Back-end Sênior", Empresa = "Empresa N", Periodo = "Mar/2019 – Atual", Descricao = "Desenvolvimento de APIs em C# e .NET, mensageria e integração com serviços em nuvem." },
                    new() { Cargo = "Desenvolvedor Pleno", Empresa = "Empresa O", Periodo = "Jan/2015 – Fev/2019", Descricao = "Manutenção de sistemas legados e migração para arquitetura de microsserviços." }
                },
                Habilidades = new() { "C#", ".NET", "MySQL", "Docker", "Azure", "APIs REST" }
            },
            new Curriculo
            {
                Nome = "Camila Rocha",
                Cargo = "Product Owner",
                Area = "Produto",
                Formacao = "Administração",
                FormacaoDetalhada = "Administração – Bacharelado",
                Instituicao = "Fundação Getulio Vargas (FGV)",
                ConclusaoFormacao = "Dez/2016",
                Idade = 31,
                AnosExperiencia = 8,
                FotoUrl = "https://i.pravatar.cc/150?img=44",
                Email = "camila.rocha@email.com",
                Telefone = "(11) 98123-4455",
                Cidade = "Campinas, SP",
                Linkedin = "linkedin.com/in/camilarocha",
                Resumo = "Product Owner com experiência em produtos digitais, métricas e metodologias ágeis.",
                Status = StatusCurriculo.EmAnalise,
                DataCadastro = DiasAtras(16),
                Experiencias = new()
                {
                    new() { Cargo = "Product Owner", Empresa = "Empresa Q", Periodo = "Jan/2020 – Atual", Descricao = "Gestão de backlog, priorização e acompanhamento de indicadores de produto." }
                },
                Habilidades = new() { "Scrum", "Kanban", "Jira", "Métricas de Produto", "Comunicação" }
            },
            new Curriculo
            {
                Nome = "Diego Martins",
                Cargo = "Analista de Suporte",
                Area = "Tecnologia da Informação",
                Formacao = "Redes de Computadores",
                FormacaoDetalhada = "Redes de Computadores – Tecnólogo",
                Instituicao = "Fatec São Paulo",
                ConclusaoFormacao = "Jun/2023",
                Idade = 22,
                AnosExperiencia = 1,
                FotoUrl = "https://i.pravatar.cc/150?img=60",
                Email = "diego.martins@email.com",
                Telefone = "(11) 94567-8899",
                Cidade = "São Paulo, SP",
                Linkedin = "linkedin.com/in/diegomartins",
                Resumo = "Analista de suporte com foco em atendimento ao usuário, redes e infraestrutura.",
                Status = StatusCurriculo.Reprovado,
                DataCadastro = DiasAtras(35),
                Experiencias = new()
                {
                    new() { Cargo = "Analista de Suporte Júnior", Empresa = "Empresa R", Periodo = "Jul/2023 – Atual", Descricao = "Atendimento de chamados, manutenção de computadores e configuração de redes." }
                },
                Habilidades = new() { "Redes", "Windows Server", "Help Desk", "Linux" }
            },
            new Curriculo
            {
                Nome = "Beatriz Lima",
                Cargo = "Social Media",
                Area = "Marketing",
                Formacao = "Jornalismo",
                FormacaoDetalhada = "Jornalismo – Bacharelado",
                Instituicao = "Universidade Federal da Bahia (UFBA)",
                ConclusaoFormacao = "Dez/2021",
                Idade = 17,
                AnosExperiencia = 0,
                FotoUrl = "https://i.pravatar.cc/150?img=5",
                Email = "beatriz.lima@email.com",
                Telefone = "(71) 99345-6677",
                Cidade = "Salvador, BA",
                Linkedin = "linkedin.com/in/beatrizlima",
                Resumo = "Jovem aprendiz com interesse em redes sociais, produção de conteúdo e fotografia.",
                Status = StatusCurriculo.Novo,
                DataCadastro = DiasAtras(40),
                Experiencias = new(),
                Habilidades = new() { "Canva", "Instagram", "Fotografia", "Redação" }
            },
            new Curriculo
            {
                Nome = "Lucas Ferreira",
                Cargo = "Designer UI/UX",
                Area = "Design",
                Formacao = "Design Digital",
                FormacaoDetalhada = "Design Digital – Bacharelado",
                Instituicao = "Universidade do Vale do Rio dos Sinos (Unisinos)",
                ConclusaoFormacao = "Dez/2019",
                Idade = 30,
                AnosExperiencia = 6,
                FotoUrl = "https://i.pravatar.cc/150?img=59",
                Email = "lucas.ferreira@email.com",
                Telefone = "(51) 98222-3344",
                Cidade = "Porto Alegre, RS",
                Linkedin = "linkedin.com/in/lucasferreira",
                Resumo = "Designer de interfaces com foco em pesquisa com usuários, prototipação e design systems.",
                Status = StatusCurriculo.EmAnalise,
                DataCadastro = DiasAtras(50),
                Experiencias = new()
                {
                    new() { Cargo = "Designer UI/UX", Empresa = "Empresa U", Periodo = "Fev/2020 – Atual", Descricao = "Pesquisa, prototipação e manutenção do design system da empresa." }
                },
                Habilidades = new() { "Figma", "Design System", "Pesquisa com Usuários", "Prototipação" }
            }
        };
    }
}
