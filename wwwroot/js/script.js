// Toda a regra de negócio fica no C# (controllers e repositórios).
// Este arquivo só cuida de pequenos comportamentos da interface.
document.addEventListener('DOMContentLoaded', () => {
    // Selects de filtro: envia o formulário assim que muda a opção
    document.querySelectorAll('select[data-auto-submit]').forEach((select) => {
        select.addEventListener('change', () => select.form?.submit());
    });

    // Pede confirmação antes de ações perigosas (ex.: excluir currículo)
    document.querySelectorAll('form[data-confirmar]').forEach((form) => {
        form.addEventListener('submit', (event) => {
            if (!window.confirm(form.dataset.confirmar)) {
                event.preventDefault();
            }
        });
    });

    // Menus suspensos (<details>): abre um de cada vez e fecha ao clicar fora
    const menus = Array.from(document.querySelectorAll('details.menu-acoes, details.notificacoes, details.mais-filtros'));

    menus.forEach((menu) => {
        menu.addEventListener('toggle', () => {
            if (!menu.open) return;
            menus.forEach((outro) => {
                if (outro !== menu) outro.open = false;
            });
        });
    });

    document.addEventListener('click', (event) => {
        menus.forEach((menu) => {
            if (menu.open && !menu.contains(event.target)) menu.open = false;
        });
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') menus.forEach((menu) => { menu.open = false; });
    });

    // Página de PDF: abre a janela de impressão ("Salvar como PDF")
    const botaoImprimir = document.querySelector('[data-imprimir]');
    if (botaoImprimir) {
        botaoImprimir.addEventListener('click', () => window.print());
        window.setTimeout(() => window.print(), 600);
    }

    // Foto que não carregou: remove a imagem e deixa aparecer as iniciais
    document.querySelectorAll('.avatar img').forEach((img) => {
        const remover = () => img.remove();
        if (img.complete && img.naturalWidth === 0) remover();
        else img.addEventListener('error', remover);
    });

    // Esconde a mensagem de sucesso depois de alguns segundos
    const toast = document.querySelector('.toast');
    if (toast) {
        window.setTimeout(() => toast.classList.add('saindo'), 4000);
    }
});
