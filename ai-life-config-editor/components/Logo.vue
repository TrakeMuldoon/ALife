<template lang="pug">
  .AILifeLogo
</template>

<style lang="scss">
$dim: 12.5vw;
$eyeScale: 4.5;
$eyeDim: $dim / $eyeScale;
$animationDelay: 2.2s;

.AILifeLogo {
  display: inline-block;
  @include circle($dim);
  background: firebrick;

  @include after {
    position: absolute;
    top: -$eyeDim / 2;
    left: calc(50% - #{$eyeDim / 2});
    transform-origin: ($eyeDim / 2) (($dim + $eyeDim) / 2);
    transform: rotate(135deg);
    @include circle($eyeDim);
    background: blue;
    clip-path: inset($eyeDim $eyeDim $eyeDim $eyeDim);
    animation: blink 5s linear forwards $animationDelay,
               spin 5s ease forwards calc(#{$animationDelay} + 2s);
  }
}

@keyframes blink {
  $blink-frames: zip(
    0     20   30    40    45    55    65    100,
    0.51  0.4  0.4   0.51  0.4   0.4   0.51  0
  );

  @each $frame, $maskMultiplier in $blink-frames {
    #{$frame}% {
      clip-path: inset($eyeDim * $maskMultiplier 0 $eyeDim * $maskMultiplier 0);
    }
  }
}

@keyframes spin {
  $spin-frames: zip(
    0       12.5      25      37.5    45      100,
    135deg  160deg    110deg  160deg  160deg  -45deg
  );

  @each $frame, $rotation in $spin-frames {
    #{$frame}% {
      transform: rotate($rotation);
    }
  }
}
</style>
