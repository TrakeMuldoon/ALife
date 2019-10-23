<template lang="pug">
	section.app-layout
		logo.logo/
		header.header
		section.sidebar
		nuxt.body/
</template>

<script>
	import Logo from '~/components/Logo.vue';

	export default {
		components: {
			Logo
		}
	}
</script>

<style lang="scss">
	$minSidebar: 46px;
	$maxSidebar: 240px;

	.app-layout {
		display: grid;
		height: 100vh;
		grid-template: {
			rows: 40px auto;
			columns: $minSidebar ($maxSidebar - $minSidebar) repeat(4, 4fr);
			areas:
				"home header header header header header"
				"sidebar body body body body body";
		}

		&.expanded {
			grid-template-areas:
				"home header header header header header"
				"sidebar sidebar body body body body";
		}
	}

	.logo {
		grid-area: home;
		align-self: center;
		
		$dim: 26px;
		$eyeDim: $dim / 4.5;

		display: inline-block;
		@include circle($dim);
		background: firebrick;

		@include after {
			position: absolute;
			top: -$eyeDim / 2;
			left: calc(50% - #{$eyeDim / 2});
			transform-origin: ($eyeDim / 2) (($dim + $eyeDim) / 2);
			transform: rotate(-45deg);
			@include circle($eyeDim);
			background: blue;
		}
	}

	.header {
		background: red;
		grid-area: header;
	}

	.sidebar {
		background: blue;
		grid-area: sidebar;
	}

	.body {
		grid-area: body;
	}
</style>