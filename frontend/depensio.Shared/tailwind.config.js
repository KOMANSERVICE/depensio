/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./**/*.{razor,html,cshtml,js}"],
    safelist: [
        'w-64',
        'w-1/2',
        'rounded-l-lg',
        'rounded-r-lg',
        'bg-gray-200',
        'grid-cols-4',
        'grid-cols-7',
        'h-6',
        'leading-6',
        'h-9',
        'leading-9',
        'shadow-lg',
        'bg-opacity-50',
        'dark:bg-opacity-80'
    ],
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                primary: { "50": "#FEF9F3", "100": "#F9EEE1", "200": "#F6E3CF", "300": "#F2D8BB", "400": "#EECBA4", "500": "#E7B983", "600": "#DC9F51", "700": "#825513", "800": "#734700", "900": "#895C1C" }
            },
            fontFamily: {
                'sans': ['Inter', 'ui-sans-serif', 'system-ui', '-apple-system', 'system-ui', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'Noto Sans', 'sans-serif', 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji'],
                'body': ['Inter', 'ui-sans-serif', 'system-ui', '-apple-system', 'system-ui', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'Noto Sans', 'sans-serif', 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji'],
                'mono': ['ui-monospace', 'SFMono-Regular', 'Menlo', 'Monaco', 'Consolas', 'Liberation Mono', 'Courier New', 'monospace']
            },
            transitionProperty: {
                'width': 'width'
            },
            textDecoration: ['active'],
            minWidth: {
                'kanban': '28rem'
            },
        },
    },
    //variants: {
    //    extend: {
    //        boxShadow: ["active", "hover"],
    //    },
    //},
    plugins: [],
}