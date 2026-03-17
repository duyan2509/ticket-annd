<template>
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 p-8">
        <div class="max-w-4xl mx-auto">
            <div class="bg-white rounded-lg shadow-lg p-8">
                <h1 class="text-4xl font-bold text-gray-800 mb-4">Welcome to TicketAnnd</h1>
                <p class="text-gray-600 text-lg mb-8">A modern ticket management system for your organization.</p>

                <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
                    <div class="bg-blue-50 rounded-lg p-6 border-l-4 border-blue-500">
                        <h3 class="text-xl font-semibold text-gray-800 mb-2">Tickets</h3>
                        <p class="text-gray-600">Manage and track your support tickets efficiently</p>
                    </div>
                    <div class="bg-green-50 rounded-lg p-6 border-l-4 border-green-500">
                        <h3 class="text-xl font-semibold text-gray-800 mb-2">Teams</h3>
                        <p class="text-gray-600">Organize your team and manage permissions</p>
                    </div>
                    <div class="bg-purple-50 rounded-lg p-6 border-l-4 border-purple-500">
                        <h3 class="text-xl font-semibold text-gray-800 mb-2">SLA</h3>
                        <p class="text-gray-600">Define and monitor service level agreements</p>
                    </div>
                </div>

                <div class="mt-12">
                    <h2 class="text-2xl font-bold text-gray-800 mb-6">How it works</h2>
                    <ul class="space-y-4">
                        <li class="flex items-center gap-4">
                            <span class="bg-blue-500 text-white rounded-full w-8 h-8 flex items-center justify-center">1</span>
                            <span class="text-gray-700">Create tickets to report issues or requests</span>
                        </li>
                        <li class="flex items-center gap-4">
                            <span class="bg-blue-500 text-white rounded-full w-8 h-8 flex items-center justify-center">2</span>
                            <span class="text-gray-700">Assign tickets to team members</span>
                        </li>
                        <li class="flex items-center gap-4">
                            <span class="bg-blue-500 text-white rounded-full w-8 h-8 flex items-center justify-center">3</span>
                            <span class="text-gray-700">Track progress and monitor SLA compliance</span>
                        </li>
                        <li class="flex items-center gap-4">
                            <span class="bg-blue-500 text-white rounded-full w-8 h-8 flex items-center justify-center">4</span>
                            <span class="text-gray-700">Close and analyze your tickets</span>
                        </li>
                    </ul>

                    <div class="text-center mt-12 flex gap-4 justify-center">
                        <NuxtLink to="/login" class="inline-block mt-4 px-6 py-3 bg-blue-600 text-white rounded hover:bg-blue-700 transition">
                            Login
                        </NuxtLink>
                        <NuxtLink to="/register" class="inline-block mt-4 px-6 py-3 bg-green-600 text-white rounded hover:bg-green-700 transition">
                            Register
                        </NuxtLink>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
definePageMeta({
    layout: 'auth',
})
</script>

