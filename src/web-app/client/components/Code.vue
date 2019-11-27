<template lang="pug">
    
    tr
        //- Code
        td {{ code.stringValue }}

        //- Status
        td
            span.badge.badge-primary(v-if='code.state === "Active"') Active
            span.badge.badge-secondary(v-else) Inactive

        //- Expiration
        td {{ code.dateExpires }}

        //- Deactivate
        td
            button.btn.btn-sm(
                v-bind:disabled='code.state !== "Active"'
                v-bind:class="{'btn-outline-danger': code.state === 'Active', 'btn-secondary': code.state !== 'Active'}"
                v-on:click='DeactivateCode()') Deactivate

</template>

<script>

import Code from './Code';

module.exports = {
    data: function() {
        return {
        }
    },
    props: ['code'],
    methods: {
        DeactivateCode() {
            this.code.state = 'Inactive';
        }
    },
}
</script>