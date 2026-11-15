document.querySelector('.js-sidebar-toggle')?.addEventListener('click', () => {
    document.getElementById('sidebar')
        .classList.toggle(window.innerWidth < 992 ? 'show-sidebar' : 'collapsed-sidebar');
});