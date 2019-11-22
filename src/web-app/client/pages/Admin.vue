<template lang="pug">

    code-table

</template>

<script>

import CodeTable from '../components/CodeTable';

module.exports = {
    name: 'Admin',
    data: function() {
        return {
            codes: [
                { id: 0, text: 'af6356', expiration: 'Date', isActive: true },
                { id: 1, text: 'bf6356', expiration: 'Date', isActive: false },
                { id: 2, text: 'cf6356', expiration: 'Date', isActive: true },
                { id: 3, text: 'df6356', expiration: 'Date', isActive: true },
            ],
        }
    },
    components: {
        CodeTable
    }
}
</script>