<template lang="pug">

    code-table

</template>

<script>

import CodeTable from '../components/CodeTable';

module.exports = {
    name: 'Admin',
    data: function() {
        return {
            codes: [],
        }
    },
    components: {
        CodeTable
    },
}
</script>