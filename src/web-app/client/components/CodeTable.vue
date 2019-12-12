<template lang="pug">
    .table-responsive
        table.table.table-bordered(width='100%' cellspacing='0')
            thead
                tr
                    th Code
                    th Status
                    th Expiration
                    th Deactivate
            tfoot
                tr
                    th Code
                    th Status
                    th Expiration
                    th Deactivate

            tbody
                Code(v-for='code in codes' :code='code' :key='code.id')
</template>

<script>

import Code from './Code';

    module.exports = {
        data: function() {
            return {
            }
        },
        props:{
            codes: {
                type: Array,
                required: true,
                default: []
            }
        },
        components: {
            Code
        },
    }

</script>