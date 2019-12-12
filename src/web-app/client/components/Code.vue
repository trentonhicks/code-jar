<template lang="pug">
    
    tr

        //- Code
        td {{ code.stringValue }}

        //- Status
        td
            span.badge.badge-primary(v-if='code.state === "Active"') Active
            span.badge.badge-success(v-else-if='code.state==="Redeemed"') Redeemed
            span.badge.badge-danger(v-else-if='code.state==="Expired"') Expired
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
        return {}
    },
    props: ['code'],
    methods: {
        DeactivateCode() {
            axios({
                method: 'delete',
                url: 'http://localhost:5000/codes',
                data: [this.code.stringValue],
                headers: {
                    'Content-Type': 'application/json'
                }
            }).then(response => {
                this.code.state = "Inactive";
            }).catch(e => {
                // Error
            });
        },
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
    },
}

</script>

<style lang="scss" scoped>

    .checkbox-wrapper {
        display: block;
        position: relative;
        padding-left: 35px;
        margin-bottom: 18px;
        cursor: pointer;
        font-size: 22px;
        -webkit-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        user-select: none;
       

        .checkbox-input {
            position: absolute;
            opacity: 0;
            cursor: pointer;
            height: 0;
            width: 0;
        }

        .checkbox {
            display: block;
            position: absolute;
            top: 0;
            left: 0;
            height: 18px;
            width: 18px;
            background-color: #ccc;
            border-radius: .2rem;
            
        }

        &:hover .checkbox-input ~ .checkbox {
              background-color: #bbb;
        }

        .checkbox-input:checked ~ .checkbox {
            background-color: #2196F3;
        }

        .checkbox:after {
            content: "";
            position: absolute;
            display: none;
        }

        .checkbox-input:checked ~ .checkbox:after {
            display: block;
        }

        .checkbox:after {
           left: 7px;
            top: 4px;
            width: 5px;
            height: 10px;
            border: solid white;
            border-width: 0 3px 3px 0;
            -webkit-transform: rotate(45deg);
            -ms-transform: rotate(45deg);
            transform: rotate(45deg);
                    }
    }
 
</style>