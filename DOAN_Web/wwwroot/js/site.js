// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Site-wide JavaScript for Shopjoy

// Update cart count on page load
$(document).ready(function() {
    updateCartCount();
    initStickyNavbar();
    initCategoryDropdown();
});

// Sticky navbar with auto-hide on scroll
function initStickyNavbar() {
    let lastScrollTop = 0;
    const navbar = document.getElementById('mainHeader');
    const delta = 5;
    let didScroll = false;

    if (!navbar) return;

    $(window).scroll(function() {
        didScroll = true;
    });

    setInterval(function() {
        if (didScroll) {
            hasScrolled();
            didScroll = false;
        }
    }, 250);

    function hasScrolled() {
        const st = $(window).scrollTop();
        
        // Make sure they scroll more than delta
        if (Math.abs(lastScrollTop - st) <= delta) return;
        
        // If scrolled down and past the navbar, add class .nav-hidden
        if (st > lastScrollTop && st > navbar.offsetHeight) {
            // Scroll Down
            navbar.classList.add('nav-hidden');
            closeCategoryDropdown();
        } else {
            // Scroll Up
            if (st + $(window).height() < $(document).height()) {
                navbar.classList.remove('nav-hidden');
            }
        }
        
        lastScrollTop = st;
    }
}

// Category dropdown functionality
function initCategoryDropdown() {
    let categoriesLoaded = false;
    const dropdown = $('#categoryDropdown');
    const trigger = $('#categoryDropdownTrigger');
    const closeBtn = $('#closeCategoryDropdown');
    
    // Only initialize if elements exist
    if (!dropdown.length || !trigger.length) return;
    
    // Create overlay
    if (!document.getElementById('categoryOverlay')) {
        $('body').append('<div class="category-dropdown-overlay" id="categoryOverlay"></div>');
    }
    const overlay = $('#categoryOverlay');
    
    // Toggle dropdown
    trigger.on('click', function(e) {
        e.preventDefault();
        
        if (!categoriesLoaded) {
            loadCategories();
            categoriesLoaded = true;
        }
        
        toggleCategoryDropdown();
    });
    
    // Close dropdown
    closeBtn.on('click', closeCategoryDropdown);
    overlay.on('click', closeCategoryDropdown);
    
    // Close on escape key
    $(document).on('keydown', function(e) {
        if (e.key === 'Escape' && dropdown.hasClass('show')) {
            closeCategoryDropdown();
        }
    });
}

function toggleCategoryDropdown() {
    const dropdown = $('#categoryDropdown');
    const overlay = $('#categoryOverlay');
    
    dropdown.toggleClass('show');
    overlay.toggleClass('show');
    
    // Disable body scroll when dropdown is open
    if (dropdown.hasClass('show')) {
        $('body').css('overflow', 'hidden');
    } else {
        $('body').css('overflow', '');
    }
}

function closeCategoryDropdown() {
    const dropdown = $('#categoryDropdown');
    const overlay = $('#categoryOverlay');
    
    dropdown.removeClass('show');
    overlay.removeClass('show');
    $('body').css('overflow', '');
}

function loadCategories() {
    $.ajax({
        url: '/api/categories',
        type: 'GET',
        success: function(categories) {
            renderCategories(categories);
        },
        error: function() {
            // Fallback: try to get categories from the page
            $('#categoriesContainer').html(
                '<div class="col-12 text-center py-3">' +
                '<p class="text-muted">Không thể tải danh mục. Vui lòng thử lại sau.</p>' +
                '<a href="/danh-muc" class="btn btn-primary btn-sm">Xem tất cả danh mục</a>' +
                '</div>'
            );
        }
    });
}

function renderCategories(categories) {
    const container = $('#categoriesContainer');
    container.empty();
    
    if (!categories || categories.length === 0) {
        container.html('<div class="col-12 text-center py-3"><p class="text-muted">Chưa có danh mục nào</p></div>');
        return;
    }
    
    categories.forEach(function(category) {
        const productCount = category.productCount || 0;
        const bgImage = category.backgroundImageUrl || '/images/default-category.jpg';
        const card = `
            <div class="col-lg-2 col-md-3 col-sm-4 col-6">
                <a href="/danh-muc/${category.slug}" class="text-decoration-none">
                    <div class="category-mini-card" style="background-image: url('${bgImage}');">
                        <div class="category-card-content">
                            <h6 class="card-title fw-bold mb-0">${category.name}</h6>
                            <small class="text-muted">${productCount} sản phẩm</small>
                        </div>
                    </div>
                </a>
            </div>
        `;
        container.append(card);
    });
    
    // Add "View All" card
    container.append(`
        <div class="col-lg-2 col-md-3 col-sm-4 col-6">
            <a href="/danh-muc" class="text-decoration-none">
                <div class="category-mini-card">
                    <div class="category-card-content">
                        <i class="bi bi-grid-3x3-gap"></i>
                        <h6 class="card-title fw-bold mb-0">Xem tất cả</h6>
                        <small class="text-muted">${categories.length} danh mục</small>
                    </div>
                </div>
            </a>
        </div>
    `);
}

function updateCartCount() {
    $.get('/cart/count', function(data) {
        $('#cart-count').text(data.count || 0);
    }).fail(function() {
        $('#cart-count').text('0');
    });
}

function showToast(message, type = 'success') {
    var bgClass = type === 'success' ? 'bg-success' : 'bg-danger';
    var toast = $('<div class="position-fixed bottom-0 end-0 p-3" style="z-index: 11">' +
        '<div class="toast show ' + bgClass + ' text-white" role="alert">' +
        '<div class="toast-body">' + message + '</div></div></div>');
    
    $('body').append(toast);
    
    setTimeout(function() {
        toast.fadeOut(function() {
            $(this).remove();
        });
    }, 3000);
}

// CSRF Token helper
function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// Product Sticky Panel Functions
window.initProductStickyPanel = function() {
    const panel = document.getElementById('productStickyPanel');
    if (!panel) return;
    
    // Show panel after page loads with smooth animation
    setTimeout(() => {
        panel.classList.add('show');
    }, 50);
    
    // Handle scroll behavior for footer collision
    let ticking = false;
    
    function updatePanelPosition() {
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        const navbar = document.getElementById('mainHeader');
        const footer = document.querySelector('footer');
        const navbarHeight = navbar ? navbar.offsetHeight : 72;
        
        if (footer && panel.classList.contains('show')) {
            const footerRect = footer.getBoundingClientRect();
            const windowHeight = window.innerHeight;
            
            // Calculate if footer is approaching
            if (footerRect.top < windowHeight && footerRect.top > 0) {
                // Footer is visible, position panel above it
                const distanceFromBottom = windowHeight - footerRect.top + 20; // 20px margin
                panel.style.bottom = `${distanceFromBottom}px`;
                panel.style.top = 'auto';
                panel.classList.add('footer-collision');
            } else {
                // Normal sticky behavior
                panel.style.top = `${navbarHeight}px`;
                panel.style.bottom = 'auto';
                panel.classList.remove('footer-collision');
            }
        }
        
        ticking = false;
    }
    
    function requestTick() {
        if (!ticking) {
            requestAnimationFrame(updatePanelPosition);
            ticking = true;
        }
    }
    
    // Throttled scroll listener
    window.addEventListener('scroll', requestTick);
    window.addEventListener('resize', requestTick);
    
    // Initial position check
    updatePanelPosition();
};
