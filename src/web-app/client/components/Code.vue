<template lang="pug">
    
    tr
        //- Code
        td {{ code.stringValue }}

        //- Status
        td
            span.badge.badge-primary(v-if='code.state === "Active"') Active
            span.badge.badge-success(v-else-if='code.state==="Redeemed"') Redeemed
            span.badge.badge-secondary(v-else) Inactive

        //- Expiration
        td {{ code.dateExpires | formatDate }}

        //- Deactivate
        td
            button.btn.btn-sm(
                v-bind:disabled='code.state !== "Active"'
                v-bind:class="{'btn-outline-danger': code.state === 'Active', 'btn-secondary': code.state !== 'Active'}"
                v-on:click='DeactivateCode()') Deactivate
        

</template>

<script>

import axios from 'axios';
import Code from './Code';

module.exports = {
    data: function() {
        return {
        }
    },
    props: ['code'],
    methods: {
        DeactivateCode() {
            axios({
                method: 'delete',
                url: 'http://localhost:5000/codes',
                data: this.code.id,
                headers: {
                    'Content-Type': 'application/json'
                }
            }).then(response => {
                this.code.state = "Inactive";
            }).catch(e => {
                // Error
            });
        }
    },
    filters: {
        formatDate: function(dateString) {
            var date = new Date(dateString);
            var hours = date.getHours();
            var minutes = date.getMinutes();
            var ampm = hours >= 12 ? 'pm' : 'am';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            minutes = minutes < 10 ? '0'+minutes : minutes;
            var strTime = hours + ':' + minutes + ' ' + ampm;
            return date.getMonth()+1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
        }
    }
}

</script>