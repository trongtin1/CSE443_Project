// Admin Dashboard JavaScript

// Toggle dropdown menus
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all dropdowns
    var dropdowns = document.querySelectorAll('.dropdown-toggle');
    dropdowns.forEach(function(dropdown) {
        dropdown.addEventListener('click', function(e) {
            e.preventDefault();
            var menu = this.nextElementSibling;
            if (menu.classList.contains('show')) {
                menu.classList.remove('show');
            } else {
                // Close all other dropdowns
                document.querySelectorAll('.dropdown-menu.show').forEach(function(openMenu) {
                    openMenu.classList.remove('show');
                });
                menu.classList.add('show');
            }
        });
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
        if (!e.target.matches('.dropdown-toggle')) {
            document.querySelectorAll('.dropdown-menu.show').forEach(function(menu) {
                menu.classList.remove('show');
            });
        }
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-toggle="popover"]'));
    popoverTriggerList.map(function(popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Handle form submissions
    var forms = document.querySelectorAll('form');
    forms.forEach(function(form) {
        form.addEventListener('submit', function(e) {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });

    // Handle delete confirmations
    var deleteButtons = document.querySelectorAll('[data-confirm]');
    deleteButtons.forEach(function(button) {
        button.addEventListener('click', function(e) {
            if (!confirm(this.dataset.confirm)) {
                e.preventDefault();
            }
        });
    });

    // Handle sidebar toggle
    var sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function(e) {
            e.preventDefault();
            document.body.classList.toggle('sidebar-toggled');
            document.querySelector('.sidebar').classList.toggle('toggled');
        });
    }

    // Handle responsive sidebar
    function handleResponsiveSidebar() {
        if (window.innerWidth < 768) {
            document.body.classList.add('sidebar-toggled');
            document.querySelector('.sidebar').classList.add('toggled');
        } else {
            document.body.classList.remove('sidebar-toggled');
            document.querySelector('.sidebar').classList.remove('toggled');
        }
    }

    // Initial check
    handleResponsiveSidebar();

    // Check on resize
    window.addEventListener('resize', handleResponsiveSidebar);

    // Handle active menu items
    var currentPath = window.location.pathname;
    var menuItems = document.querySelectorAll('.nav-link');
    menuItems.forEach(function(item) {
        if (item.getAttribute('href') === currentPath) {
            item.classList.add('active');
        }
    });

    // Handle flash messages
    var flashMessages = document.querySelectorAll('.alert');
    flashMessages.forEach(function(message) {
        setTimeout(function() {
            message.style.opacity = '0';
            setTimeout(function() {
                message.remove();
            }, 300);
        }, 3000);
    });
}); 