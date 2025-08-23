window.depensio = window.depensio || {};

window.depensio.initialized = function () {

    // Mobile menu toggle
    const mobileMenuButton = document.getElementById('mobile-menu-button');
    const mobileMenu = document.getElementById('mobile-menu');

    if (mobileMenuButton && mobileMenu) {
        mobileMenuButton.addEventListener('click', () => {
            mobileMenu.classList.toggle('hidden');
        });
    }

    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({ behavior: 'smooth' });

                // Close mobile menu if open
                if (mobileMenu && !mobileMenu.classList.contains('hidden')) {
                    mobileMenu.classList.add('hidden');
                }
            }
        });
    });

    // Chatbot
    const chatbotIcon = document.getElementById('chatbotIcon');
    const chatbotModal = document.getElementById('chatbotModal');
    const closeChatbot = document.getElementById('closeChatbot');
    const chatMessages = document.getElementById('chatMessages');
    const chatInput = document.getElementById('chatInput');
    const sendMessage = document.getElementById('sendMessage');

    function addMessage(message, isUser = false) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `flex mb-4 ${isUser ? 'justify-end' : ''}`;

        if (!isUser) {
            messageDiv.innerHTML = `
                <div class="w-8 h-8 bg-primary-700 rounded-full flex items-center justify-center text-white mr-3">A</div>
                <div class="bg-white p-3 rounded-lg shadow-sm max-w-xs">
                    <p class="text-gray-800">${message}</p>
                </div>`;
        } else {
            messageDiv.innerHTML = `
                <div class="bg-primary-700 text-white p-3 rounded-lg shadow-sm max-w-xs">
                    <p>${message}</p>
                </div>
                <div class="w-8 h-8 bg-gray-300 rounded-full flex items-center justify-center text-gray-600 ml-3">V</div>`;
        }

        chatMessages?.appendChild(messageDiv);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    if (chatbotIcon && chatbotModal) {
        chatbotIcon.addEventListener('click', () => {
            chatbotModal.classList.toggle('hidden');
        });
    }

    if (closeChatbot && chatbotModal) {
        closeChatbot.addEventListener('click', () => {
            chatbotModal.classList.add('hidden');
        });
    }

    if (sendMessage && chatInput) {
        sendMessage.addEventListener('click', () => {
            const message = chatInput.value.trim();
            if (message) {
                addMessage(message, true);
                chatInput.value = '';

                // Fake response
                setTimeout(() => {
                    const responses = [
                        "Je comprends votre demande. Un membre de notre équipe vous contactera bientôt.",
                        "Merci pour votre message! Comment puis-je vous aider davantage?",
                        "J'ai noté votre question. Nous y répondrons dans les plus brefs délais.",
                        "Pourriez-vous préciser votre demande s'il vous plaît?"
                    ];
                    const randomResponse = responses[Math.floor(Math.random() * responses.length)];
                    addMessage(randomResponse);
                }, 1000);
            }
        });

        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                sendMessage.click();
            }
        });
    }

    // Form submission
    const contactForm = document.getElementById('contactForm');
    if (contactForm) {
        contactForm.addEventListener('submit', (e) => {
            e.preventDefault();

            const name = document.getElementById('name')?.value;
            const email = document.getElementById('email')?.value;
            const subject = document.getElementById('subject')?.value;
            const message = document.getElementById('message')?.value;

            console.log({ name, email, subject, message });
            alert('Merci pour votre message! Nous vous contacterons bientôt.');
            contactForm.reset();
        });
    }

    const scanTab = document.getElementById('scan-tab');
    const manualTab = document.getElementById('manual-tab');
    const scanSection = document.getElementById('scan-section');
    const manualSection = document.getElementById('manual-section');
    const scannerAnimation = document.querySelector('.scanner-animation');
    const barcodeInput = document.getElementById('barcode-input');

    if (scanTab && manualTab && scanSection && manualSection && scannerAnimation && barcodeInput) {

        // Tab switching
        scanTab.addEventListener('click', function () {
            this.classList.add('primary-text', 'border-b-2', 'primary-border');
            manualTab.classList.remove('primary-text', 'border-b-2', 'primary-border');
            manualTab.classList.add('text-gray-500');
            scanSection.classList.remove('hidden');
            manualSection.classList.add('hidden');
        });

        manualTab.addEventListener('click', function () {
            this.classList.add('primary-text', 'border-b-2', 'primary-border');
            scanTab.classList.remove('primary-text', 'border-b-2', 'primary-border');
            scanTab.classList.add('text-gray-500');
            manualSection.classList.remove('hidden');
            scanSection.classList.add('hidden');
        });

        // Focus barcode input when scanner area is clicked
        scannerAnimation.addEventListener('click', function () {
            barcodeInput.focus();
        });
    }
}
