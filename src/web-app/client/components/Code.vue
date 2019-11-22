<template lang="pug">
    
    tr
        //- Code
        td {{ code.text }}

        //- Status
        td
            span.badge.badge-primary(v-if='code.isActive') Active
            span.badge.badge-secondary(v-else='') Inactive

        //- Expiration
        td {{ code.expiration }}

        //- Deactivate
        td
            button.btn.btn-sm(
                v-bind:disabled='!code.isActive'
                v-bind:class="{'btn-outline-danger': code.isActive, 'btn-secondary': !code.isActive}"
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
            this.code.isActive = false;
            console.log(`Deactivate code ${this.code.id}`);
        }
    },
}
</script>