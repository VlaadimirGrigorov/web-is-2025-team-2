import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { resolve } from 'path'

export default defineConfig({
    plugins: [react()],
    build: {
        rollupOptions: {
            input: {
                main: resolve(__dirname, 'login-page/index.html'),
            },
        },
    },
    server: {
        // Optional: Set the server to open the login page by default
        open: '/login-page/index.html',
    },
})
