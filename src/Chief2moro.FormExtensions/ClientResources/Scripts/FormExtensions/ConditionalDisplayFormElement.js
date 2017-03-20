
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

        //DEBUG
        //console.log(JSON.stringify(showhideInstructions));

        //For each instruction set initial state and bind an change event handler to the element to modify state
        for (var i = 0; i < showhideInstructions.length; i++) {

            //hide all affected element
            conditionalDisplayHideElements(showhideInstructions[i].Hide);
            //show all elements that have show instruction based on current form state (post-validation)
            var sourceElement = $$epiforms('#' + showhideInstructions[i].SourceElement);
            conditionalDisplaySetElements(showhideInstructions[i], sourceElement.val());
            
            //bind event handler
            $$epiforms(sourceElement).change(function () {

                var localElement = $$epiforms(this);
                var currentElementId = localElement.attr('id');
                var currentElementValue = localElement.val();

                //get instruction for current source from original show hide instructions array
                var instructionForElement = showhideInstructions.filter(function (item) {
                    return item.SourceElement === currentElementId;
                });

                conditionalDisplaySetElements(instructionForElement[0], currentElementValue);
            });
        }
    });

    function conditionalDisplaySetElements(instructionForElement, value) {

        //console.log("|CHANGE|" + value);
        //console.log("Instruction");
        //console.log(JSON.stringify(instructionForElement));

        //hide all elements in this instruction
        conditionalDisplayHideElements(instructionForElement.Hide);

        for (var j = 0; j < instructionForElement.Instructions.length; j++) {

            var localInstruction = instructionForElement.Instructions[j];
            //if current element valuematches the instruction value                   
            if (value === localInstruction.Value) {

                // console.log('Showing elements for value : ' + localInstruction.Value);
                for (var l = 0; l < localInstruction.Show.length; l++) {

                    //console.log("Showing element : " + localInstruction.Show[l]);
                    var formfield = $$epiforms('#' + localInstruction.Show[l]);
                    var wrapperName = formfield.attr("name");
                    $$epiforms('[data-epiforms-element-name=' + wrapperName + ']').show();

                    var currentFieldValue = formfield.val();

                    //HACK - if form field value is default then empty it
                    if (currentFieldValue === "N/A") {
                        //console.log("Default value :" + formfield.attr("name"));
                        formfield.val("");
                    }
                }
            }
        }
    }

    function conditionalDisplayHideElements(elementsToHide) {

        for (var n = 0; n < elementsToHide.length; n++) {
            //console.log("Hiding element : " + elementsToHide[n]);
            var formfieldToHide = $$epiforms('#' + elementsToHide[n]);
            var wrapperNameToHide = formfieldToHide.attr("name");
            //hide wrapping form field 
            $$epiforms('[data-epiforms-element-name=' + wrapperNameToHide + ']').hide();
            //HACK - set a default value to pass simple validation
            formfieldToHide.val("N/A");
        }
    }
}
