﻿
if (typeof $$epiforms !== 'undefined') {

    //use EPiforms jquery and bind function on ready
    $$epiforms(document).ready(function () {

        //Find all elements on page with data-conditional
        var conditionalShowHideElements = $$epiforms('[data-formcondition]');
        //Parse conditional isnstructions from html in each element
        var conditionalInstructions = conditionalShowHideElements.map(function () {
            return JSON.parse($$epiforms(this).html());
        });

        var showhideInstructions = [];
        
        //Combine and create data in a structure that can be bound to an event handler
        conditionalInstructions.each(function () {

            var sourceElement = this.SourceElement;
           
            //Create new show instruction
            var showInstructions = [{ Value: this.Value, Show: this.TargetElements }];

            //Find if a SourceElement with this identifier been added before
            var existingSourceElement = showhideInstructions.filter(function (item) {
                return item.SourceElement === sourceElement;
            });

            //If SourceElement with this identifier been added before
            if (existingSourceElement.length === 0) {
                
                //create new conditonal instruction and add to array
                var conditionalInstruction = { SourceElement: sourceElement, Instructions: showInstructions, Hide: this.TargetElements };
                showhideInstructions.push(conditionalInstruction);
               
            } else {

                //Add new instruction to existing SourceElement record
                existingSourceElement[0].Instructions.push({ Value: this.Value, Show: this.TargetElements });
                //combine instruction targets and get a distinct list of all elements to hide
                existingSourceElement[0].Hide = existingSourceElement[0].Hide
                    .concat(this.TargetElements)
                    .reduce(function (a, b) { if (a.indexOf(b) < 0) a.push(b); return a; }, []);
            }
        });

        console.log(JSON.stringify(showhideInstructions));

        //For each instruction bind an change event handler to the element
        for (var i = 0; i < showhideInstructions.length; i++) {

            $$epiforms('#' + showhideInstructions[i].SourceElement).change(function () {
                
                var localElement = $$epiforms(this);
                var currentElementId = localElement.attr('id');
                var currentElementValue = localElement.val();
                
                console.log('|CHANGE|' + currentElementValue);

                //get instruction for current source from original showhise instructions array
                var instructionForElement = showhideInstructions.filter(function (item) {
                    return item.SourceElement === currentElementId;
                });

                var currentInstruction = instructionForElement[0];

                //hide all elements for this source
                for (var k = 0; k < currentInstruction.Hide.length; k++) {

                    console.log("Hiding element : " + currentInstruction.Hide[k]);
                    var formfieldToHide = $$epiforms('#' + currentInstruction.Hide[k]);
                    var wrapperNameToHide = formfieldToHide.attr("name");
                    //hide wrapping form field 
                    $$epiforms('[data-epiforms-element-name=' + wrapperNameToHide + ']').hide();
                    //HACK - set a default value to pass simple validation
                    formfieldToHide.val("N/A");
                }

                for (var j = 0; j < currentInstruction.Instructions.length; j++) {

                    var localInstruction = currentInstruction.Instructions[j];
                    
                    if (currentElementValue === localInstruction.Value) {

                        console.log('Showing elements for value : ' + localInstruction.Value);

                        for (var l = 0; l < localInstruction.Show.length; l++) {

                            console.log("Showing element : " + localInstruction.Show[l]);

                            var formfield = $$epiforms('#' + localInstruction.Show[l]);
                            var wrapperName = formfield.attr("name");
                            $$epiforms('[data-epiforms-element-name=' + wrapperName + ']').show();
                            
                            var currentFieldValue = formfield.val();
                           
                            //HACK - if form field value is default then empty it
                            if (currentFieldValue === "N/A") {
                                console.log("Default value :" + formfield.attr("name"));
                                formfield.val("");
                            } else {
                                console.log("Not default value");

                            }
                        }
                    }
                }
            });
        }
    });
}
