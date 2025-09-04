vec4 rayMarch(vec3 samplePosition, vec3 marchDirection, int stepCount, float stepSize, float extinctionCoef){
    float transmit = 1.0;

    for(int i = 0; i < stepCount; i++){
        samplePosition+=marchDirection*stepSize;
        transmit *= exp(-density(samplePosition)*extinctionCoef*stepSize);
    }
    return vec4(vec3(0,0,0), transmit);
}

vec3 voxelize(float dist2frustrum, int maxSteps, int stepSize){
    int step = ceil(dist2frustrum/stepSize);
    if(step > maxSteps)
        return vec3(0,0,0);
    int vox = maxSteps - dist2frustrum;
    for(int i = 0; i < vox; i++){
        //do the thing
    }
}

real density(vec3 pos){

}

real beersLaw(){
    real distance = 0.0;
    int maxLen;
    for (int i = 0; i < maxLen; i++){
        distance += (segment*i) * density(segment*i); 
    }
    return exp(-distance, absorbtion);
}