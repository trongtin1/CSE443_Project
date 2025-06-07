// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code here.
$(document).ready(function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Auto-dismiss alerts after 5 seconds
    $('.alert-dismissible').each(function() {
        var alert = this;
        setTimeout(function() {
            $(alert).fadeOut('slow');
        }, 5000);
    });

    // Add smooth scrolling to all links
    $('a[href*="#"]').on('click', function (e) {
        var target = $(this.hash);
        if (target.length) {
            e.preventDefault();
            $('html, body').animate({
                scrollTop: target.offset().top - 70
            }, 1000);
        }
    });

    // Form validation feedback
    $('form').on('submit', function(e) {
        var form = this;
        if (form.checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
        }
        $(form).addClass('was-validated');
    });

    // Loading states for forms
    $('form').on('submit', function() {
        var submitBtn = $(this).find('button[type="submit"]');
        var originalText = submitBtn.text();
        
        submitBtn.prop('disabled', true);
        if (!submitBtn.hasClass('btn-loading-custom')) {
            submitBtn.html('<i class="fas fa-spinner fa-spin me-1"></i> Processing...');
        }
        
        // Re-enable after 10 seconds as a failsafe
        setTimeout(function() {
            submitBtn.prop('disabled', false).text(originalText);
        }, 10000);
    });

    // Handle file input labels
    $('.custom-file-input').on('change', function() {
        var fileName = $(this).val().split('\\').pop();
        $(this).next('.custom-file-label').html(fileName);
    });
});
